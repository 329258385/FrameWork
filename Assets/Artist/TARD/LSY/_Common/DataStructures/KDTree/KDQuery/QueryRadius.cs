﻿//==========================================
//Author : Luo Shengyu
//Summary: Heap Sorting
//==========================================
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DataStructures.ViliWonka.KDTree {

    public partial class KDQuery {

        /// <summary>
        /// Search by radius method.
        /// </summary>
        /// <param name="tree">Tree to do search on</param>
        /// <param name="queryPosition">Position</param>
        /// <param name="queryRadius">Radius</param>
        /// <param name="resultIndices">Initialized list, cleared.</param>
        public void Radius(KDTree tree, Vector3 queryPosition, float queryRadius, List<int> resultIndices) {

            Reset();

            Vector3[] points = tree.Points;
            int[] permutation = tree.Permutation;

            float squaredRadius = queryRadius * queryRadius;

            var rootNode = tree.RootNode;

            PushToQueue(rootNode, rootNode.bounds.ClosestPoint(queryPosition));

            KDQueryNode queryNode = null;
            KDNode node = null;

            // KD search with pruning (don't visit areas which distance is more away than range)
            // Recursion done on Stack
            while(LeftToProcess > 0) {

                queryNode = PopFromQueue();
                node = queryNode.node;

                if(!node.Leaf) {

                    int partitionAxis = node.partitionAxis;
                    float partitionCoord = node.partitionCoordinate;

                    Vector3 tempClosestPoint = queryNode.tempClosestPoint;

                    if((tempClosestPoint[partitionAxis] - partitionCoord) < 0) {

                        // we already know we are inside negative bound/node,
                        // so we don't need to test for distance
                        // push to stack for later querying

                        // tempClosestPoint is inside negative side
                        // assign it to negativeChild
                        PushToQueue(node.negativeChild, tempClosestPoint);

                        tempClosestPoint[partitionAxis] = partitionCoord;

                        float sqrDist = Vector3.SqrMagnitude(tempClosestPoint - queryPosition);

                        // testing other side
                        if(node.positiveChild.Count != 0
                        && sqrDist <= squaredRadius) {

                            PushToQueue(node.positiveChild, tempClosestPoint);
                        }
                    }
                    else {

                        // we already know we are inside positive bound/node,
                        // so we don't need to test for distance
                        // push to stack for later querying

                        // tempClosestPoint is inside positive side
                        // assign it to positiveChild
                        PushToQueue(node.positiveChild, tempClosestPoint);

                        // project the tempClosestPoint to other bound
                        tempClosestPoint[partitionAxis] = partitionCoord;

                        float sqrDist = Vector3.SqrMagnitude(tempClosestPoint - queryPosition);

                        // testing other side
                        if(node.negativeChild.Count != 0
                        && sqrDist <= squaredRadius) {

                            PushToQueue(node.negativeChild, tempClosestPoint);
                        }
                    }
                }
                else {

                    // LEAF
                    for(int i = node.start; i < node.end; i++) {

                        int index = permutation[i];

                        if(Vector3.SqrMagnitude(points[index] - queryPosition) <= squaredRadius) {

                            resultIndices.Add(index);
                        }
                    }

                }
            }
        }

    }
}