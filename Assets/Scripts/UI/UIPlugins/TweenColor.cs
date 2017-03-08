using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[AddComponentMenu("UI/Tween/Color Tween")]
public class TweenColor : Tweener
{
    #region Public Member
    public Color m_From = Color.white;
    public Color m_To = Color.white;
    #endregion Public Member

    #region Private Member
    private Image m_Image = null;
    private Material m_Material = null;
    private Light m_Light = null;
    #endregion Private Member

    #region Override MonoBehaviour
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void onUpdate()
    {
        float factor = (m_AnimationCurve != null) ? m_AnimationCurve.Evaluate(m_Factor) : m_Factor;
        value = Color.Lerp(m_From, m_To, factor);
        base.onUpdate();
    }

    protected override void SetStartToCurrentValue()
    {
        m_From = value;
    }

    protected override void SetEndToCurrentValue()
    {
        m_To = value;
    }
    protected override bool IsPlaying()
    {
        bool result = true;
        result = From != To;
        if (result)
            result = IsReverse() ? value != From : value != To;

        return result;
    }

    #endregion Override MonoBehaviour

    #region Property
    public Image Img
    {
        get
        {
            m_Image = GetComponent<Image>();
            if (m_Image == null && m_Material == null && m_Light == null)
                m_Image = GetComponentInChildren<Image>();

            return m_Image;
        }
    }

    public Material Mtrl
    {
        get
        {
            Renderer ren = GetComponent<Renderer>();
            if (ren != null)
                m_Material = ren.material;

            return m_Material;
        }
    }

    public Light Light
    {
        get
        {
            m_Light = GetComponent<Light>();
            return m_Light;
        }
    }
    public Color From
    {
        get { return m_From; }
        set { m_From = value; }
    }

    public Color To
    {
        get { return m_To; }
        set { m_To = value; }
    }

    public Color value
    {
        get
        {
            if (Img) return Img.color;
            if (Light) return Light.color;
            if (Mtrl) return Mtrl.color;

            return Color.black;
        }

        set
        {
            if (Img) Img.color = value;
            if (Mtrl) m_Material.color = value;

            if (Light)
            {
                Light.color = value;
                Light.enabled = (value.r + value.g + value.b) > 0.01f;
            }
        }
    }

    public Color color
    {
        get { return this.value; }
        set { this.value = value; }
    }
    #endregion Property

    #region Public Function
    public override void init()
    {
        base.init();
    }
    #endregion Public Function

    #region Private Function
    #endregion Private Function
}
