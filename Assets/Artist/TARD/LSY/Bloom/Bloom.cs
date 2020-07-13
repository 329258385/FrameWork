//Bloom的修改
//1 去掉High Quality
//2 统一PC/Mobile RT格式从Default和DefaultHDR统一为Default
//3 Bloom Shader算法增强色偏

//
// Kino/Bloom v2 - Bloom filter for Unity
//
// Copyright (C) 2015, 2016 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
using UnityEngine;
using System.Collections.Generic;
namespace Kino
{
	public enum BloomRTType
	{
		New,
		Temp
	}
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Kino Image Effects/Bloom")]
    public class Bloom : MonoBehaviour
    {
		#region RenderTexture Settings
		public RenderTexture prefiltered;
		const int kMaxIterations = 16;
		public RenderTexture[] _blurBuffer1;
		public RenderTexture[] _blurBuffer2;

		public static BloomRTType RT_Type = BloomRTType.New;

		public static int RT_Depth=0;
		public static RenderTextureFormat RT_Format=RenderTextureFormat.DefaultHDR;
		public static RenderTextureReadWrite RT_RW = RenderTextureReadWrite.Linear;
		public static int RT_AA=2;
		public static RenderTextureMemoryless RT_MemoryMode= RenderTextureMemoryless.None;
	

		public static void RT_AllClear()
		{
			foreach (var item in ins) {
				item.RT_Clear ();	
			}
		}
		public void RT_Clear()
		{
			ClearRT (prefiltered);
			prefiltered = null;
			if (_blurBuffer1 != null) {
				for (int i = 0; i < _blurBuffer1.Length; i++) {
					ClearRT (_blurBuffer1 [i]);
					_blurBuffer1 [i] = null;
				}
			}
			if (_blurBuffer2 != null) {
				for (int i = 0; i < _blurBuffer2.Length; i++) {
					ClearRT (_blurBuffer2 [i]);
					_blurBuffer2 [i] = null;
				}
			}
		}
		void ClearRT(RenderTexture rt)
		{
			if (rt == null)
				return;
			rt.Release ();
			DestroyImmediate (rt);
		}



		public static void RT_AllClearColor()
		{
			foreach (var item in ins) {
				item.RT_ClearColor ();	
			}
		}
		public void RT_ClearColor()
		{
			ClearOutRt (prefiltered);
			if (_blurBuffer1 != null) {
				for (int i = 0; i < _blurBuffer1.Length; i++) {
					if (_blurBuffer1 [i] != null)
						ClearOutRt (_blurBuffer1 [i]);
				}
			}
			if (_blurBuffer2 != null) {
				for (int i = 0; i < _blurBuffer2.Length; i++) {
					if (_blurBuffer2 [i] != null)
						ClearOutRt (_blurBuffer2 [i]);
				}
			}
		}
		public void ClearOutRt(RenderTexture rt)
		{
			RenderTexture act = RenderTexture.active;
			RenderTexture.active = rt;
			GL.Clear (true, true, Color.clear);
			RenderTexture.active = act;
		}
		#endregion

        #region Public Properties
		[Range(0f,1f)]
		public float ex=0.1f;
		[Range(0f,3f)]
		public float ex2=0f;
		[Range(0f,0.3f)]
		public float _SoftFocus=0f;
		public bool lockParameters = true;


        /// Prefilter threshold (gamma-encoded)
        /// Filters out pixels under this level of brightness.
        public float thresholdGamma {
            get { return Mathf.Max(_threshold, 0); }
            set { _threshold = value; }
        }

        /// Prefilter threshold (linearly-encoded)
        /// Filters out pixels under this level of brightness.
        public float thresholdLinear {
            get { return GammaToLinear(thresholdGamma); }
            set { _threshold = LinearToGamma(value); }
        }

        [SerializeField]
        [Tooltip("Filters out pixels under this level of brightness.")]
        float _threshold = 0.8f;

        /// Soft-knee coefficient
        /// Makes transition between under/over-threshold gradual.
        public float softKnee {
            get { return _softKnee; }
            set { _softKnee = value; }
        }

        [SerializeField, Range(0, 1)]
        [Tooltip("Makes transition between under/over-threshold gradual.")]
        float _softKnee = 0.5f;

        /// Bloom radius
        /// Changes extent of veiling effects in a screen
        /// resolution-independent fashion.
        public float radius {
            get { return _radius; }
            set { _radius = value; }
        }

        [SerializeField, Range(1, 7)]
        [Tooltip("Changes extent of veiling effects\n" +
                 "in a screen resolution-independent fashion.")]
        float _radius = 4f;

        /// Bloom intensity
        /// Blend factor of the result image.
        public float intensity {
            get { return Mathf.Max(_intensity, 0); }
            set { _intensity = value; }
        }

		[SerializeField,Range(0,10)]
        [Tooltip("Blend factor of the result image.")]
        float _intensity = 0.5f;

		/// High quality mode
		/// Controls filter quality and buffer resolution.
		private bool _highQuality {
			get { return false; }
		}
//        /// High quality mode
//        /// Controls filter quality and buffer resolution.
//        public bool highQuality {
//            get { return _highQuality; }
//            set { _highQuality = value; }
//        }
//        [SerializeField]
//        [Tooltip("Controls filter quality and buffer resolution.")]
//        bool _highQuality = true;

        /// Anti-flicker filter
        /// Reduces flashing noise with an additional filter.
        [SerializeField]
        [Tooltip("Reduces flashing noise with an additional filter.")]
        bool _antiFlicker = true;

        public bool antiFlicker {
            get { return _antiFlicker; }
            set { _antiFlicker = value; }
        }

        #endregion

        #region Private Members

        [SerializeField, HideInInspector]
        Shader _shader;

        Material _material;



        float LinearToGamma(float x)
        {
			return x;
//        #if UNITY_5_3_OR_NEWER
//            return Mathf.LinearToGammaSpace(x);
//        #else
//            if (x <= 0.0031308f)
//                return 12.92f * x;
//            else
//                return 1.055f * Mathf.Pow(x, 1 / 2.4f) - 0.055f;
//        #endif
        }

        float GammaToLinear(float x)
        {
			return x;
//        #if UNITY_5_3_OR_NEWER
//            return Mathf.GammaToLinearSpace(x);
//        #else
//            if (x <= 0.04045f)
//                return x / 12.92f;
//            else
//                return Mathf.Pow((x + 0.055f) / 1.055f, 2.4f);
//        #endif
        }

        #endregion


        #region MonoBehaviour Functions
		void RTInit(int iterations,int tw,int th)
		{
			if (RT_Type == BloomRTType.New)
				RTInit_New (iterations,tw,th);
			else
				RTInit_GetTemp (iterations,tw,th);
		}
		void RTInit_New(int iterations,int tw,int th)
		{
			if (prefiltered != null) {
				return;
			}
			if(_blurBuffer1==null || _blurBuffer1.Length<kMaxIterations)
				_blurBuffer1 = new RenderTexture[kMaxIterations];
			if(_blurBuffer2==null || _blurBuffer2.Length<kMaxIterations)
				_blurBuffer2 = new RenderTexture[kMaxIterations];


			prefiltered = CreateRT (tw, th, "Lsy RT-Bloom prefiltered");

			var last = prefiltered;
			for (var level = 0; level < iterations; level++)
			{
				_blurBuffer1[level] = CreateRT(last.width / 2, last.height / 2,"Lsy RT-Bloom tap");
			}

			// upsample and combine loop
			for (var level = iterations - 2; level >= 0; level--)
			{
				var basetex = _blurBuffer1[level];
				_blurBuffer2[level] = CreateRT(basetex.width / 2, basetex.height / 2,"Lsy RT-Bloom upsample");
			}
		}
		void RTInit_GetTemp(int iterations,int tw,int th)
		{
			if(_blurBuffer1==null || _blurBuffer1.Length<kMaxIterations)
				_blurBuffer1 = new RenderTexture[kMaxIterations];
			if(_blurBuffer2==null || _blurBuffer2.Length<kMaxIterations)
				_blurBuffer2 = new RenderTexture[kMaxIterations];
			
			prefiltered = GetTempRT (tw, th, "Lsy RT-Bloom prefiltered");

			var last = prefiltered;
			for (var level = 0; level < iterations; level++)
			{
				_blurBuffer1[level] = GetTempRT(last.width / 2, last.height / 2,"Lsy RT-Bloom tap");
			}

			// upsample and combine loop
			for (var level = iterations - 2; level >= 0; level--)
			{
				var basetex = _blurBuffer1[level];
				_blurBuffer2[level] = GetTempRT(basetex.width / 2, basetex.height / 2,"Lsy RT-Bloom upsample");
			}
		}

        void OnEnable()
        {
#if UNITY_EDITOR
            DestroyImmediate(this);
#else
        Destroy(this);
#endif
            var shader = _shader ? _shader : UnityUtils.FindShader("Hidden/Kino/Bloom");
            _material = new Material(shader);
            _material.hideFlags = HideFlags.DontSave;

			if (GetComponent<BloomRunTimeModifier> () == null) {
				gameObject.AddComponent<BloomRunTimeModifier> ();
			}
        }

        void OnDisable()
        {
            DestroyImmediate(_material);
        }

		bool Switch()
		{
			#if UNITY_EDITOR
				return TARDSwitchesPC.BloomSwitch();
			#else
				return TARDSwitches.BloomSwitch;
			#endif
		}

		bool firstClear = false;
		void Update()
		{
#if UNITY_EDITOR
            DestroyImmediate(this);
#else
        Destroy(this);
#endif
            //Avoid white screen
            if (RT_Type == BloomRTType.New) {
				if (!firstClear) {
					if (prefiltered != null) {
						RT_Clear ();
						firstClear = true;
					}
				}
			}
		}

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
#if UNITY_EDITOR
            DestroyImmediate(this);
#else
        Destroy(this);
#endif
            if (lockParameters) {
				_threshold = 0.9f;
				softKnee = 0.5f;
				intensity = 4f;
				radius = 4f;
				antiFlicker = true;
				ex = 1f;
				ex2 = 1.18f;
				_SoftFocus = 0.006f;
			}

			if (!Switch()) {
				Graphics.Blit(source, destination);
				return;
			}
	
//			// lsy 
//			if (TARDSwitches.BloomThreshold >= 0)
//				_threshold = TARDSwitches.BloomThreshold;
//			if (TARDSwitches.BloomIntensity >= 0)
//				_intensity = TARDSwitches.BloomIntensity;

            // source texture size
            var tw = source.width;
            var th = source.height;
			tw /= 4;
			th /= 4;
//            // halve the texture size for the low quality mode
//            if (!_highQuality)
//            {
//                tw /= 2;
//                th /= 2;
//            }


            // determine the iteration count
            var logh = Mathf.Log(th, 2) + _radius - 8;
            var logh_i = (int)logh;
            var iterations = Mathf.Clamp(logh_i, 1, kMaxIterations);

            // update the shader properties
            var lthresh = thresholdLinear;
            _material.SetFloat("_Threshold", lthresh);

            var knee = lthresh * _softKnee + 1e-5f;
            var curve = new Vector3(lthresh - knee, knee * 2, 0.25f / knee);
            _material.SetVector("_Curve", curve);

            var pfo = !_highQuality && _antiFlicker;
            _material.SetFloat("_PrefilterOffs", pfo ? -0.5f : 0.0f);

            _material.SetFloat("_SampleScale", 0.5f + logh - logh_i);
            _material.SetFloat("_Intensity", intensity);
			_material.SetFloat("_ex", ex);
			_material.SetFloat("_ex2", ex2);
			_material.SetFloat("_SoftFocus", _SoftFocus);

			RTInit (kMaxIterations, tw, th);
            var pass = _antiFlicker ? 1 : 0;
            Graphics.Blit(source, prefiltered, _material, pass);

            // construct a mip pyramid
            var last = prefiltered;
            for (var level = 0; level < iterations; level++)
            {
                pass = (level == 0) ? (_antiFlicker ? 3 : 2) : 4;
                Graphics.Blit(last, _blurBuffer1[level], _material, pass);

                last = _blurBuffer1[level];
            }

            // upsample and combine loop
            for (var level = iterations - 2; level >= 0; level--)
            {
                var basetex = _blurBuffer1[level];
                _material.SetTexture("_BaseTex", basetex);
                pass = _highQuality ? 6 : 5;
                Graphics.Blit(last, _blurBuffer2[level], _material, pass);
                last = _blurBuffer2[level];
            }

            // finish process
            _material.SetTexture("_BaseTex", source);
            pass = _highQuality ? 8 : 7;
            Graphics.Blit(last, destination, _material, pass);

			if (RT_Type == BloomRTType.Temp) {
				// release the temporary buffers
				for (var i = 0; i < kMaxIterations; i++)
				{
					if (_blurBuffer1[i] != null)
						RenderTexture.ReleaseTemporary(_blurBuffer1[i]);

					if (_blurBuffer2[i] != null)
						RenderTexture.ReleaseTemporary(_blurBuffer2[i]);

					_blurBuffer1[i] = null;
					_blurBuffer2[i] = null;
				}

				RenderTexture.ReleaseTemporary(prefiltered);
			}
        }

        #endregion


		#region Quality
		private static List<Bloom> ins = new List<Bloom> ();
		void Awake()
		{
#if UNITY_EDITOR
            DestroyImmediate(this);
#else
        Destroy(this);
#endif
            return;
            TARDSwitches.SetMSAA(GetComponent<Camera>());
            if (!ins.Contains (this)) {
				ins.Add (this);
			}
			SetQuality (TARDSwitches.Quality);
		}
		void OnDestroy()
		{
			if (ins.Contains (this)) {
				ins.Remove (this);
			}
		}
		public static void SetQualityAll(int q)
		{
			foreach (var item in ins) {
				item.SetQuality (q);	
			}
		}
		public void SetQuality(int q)
		{
			enabled = q > 0;		
		}

		public static void SetThreshold(float f)
		{
			foreach (var item in ins) {
				item.lockParameters = false;
				item._threshold = f;
			}
		}
#endregion

		public static RenderTexture CreateRT(int w,int h,string name)
		{
			var rt = new RenderTexture (w, h, RT_Depth, RT_Format, RT_RW);
			rt.antiAliasing = RT_AA;
			rt.name = name;
			rt.useMipMap = false;
			rt.anisoLevel = 0;
			rt.autoGenerateMips = false; 
			return rt;
		}

		public static RenderTexture GetTempRT(int w,int h,string name)
		{
			var rt = RenderTexture.GetTemporary(
				w, h, RT_Depth, RT_Format
				,RT_RW,RT_AA,RT_MemoryMode,VRTextureUsage.None,false);

			rt.name = name;
			rt.useMipMap = false;
			rt.anisoLevel = 0;
			rt.autoGenerateMips = false; 
			return rt;
		}
    }
}
