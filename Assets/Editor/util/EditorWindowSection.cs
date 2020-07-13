using UnityEngine;
using UnityEditor;
using System.Collections;

namespace BigWorldTool
{
    public class EditorWindowSection
    {
        public EditorWindow hostWindow;
        public bool isActive;

        public EditorWindowSection()
        {
        }

        public EditorWindowSection(EditorWindow host)
        {
            this.hostWindow = host;
        }

        public virtual void OnEnable()
        {
        }

        public virtual void OnGUI()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void OnInspectorUpdate()
        {
        }

        public virtual void OnDestroy()
        {

        }

        public virtual void OnFocus()
        {

        }

        public void RepaintHost()
        {
            hostWindow.Repaint();
        }
    }
}
