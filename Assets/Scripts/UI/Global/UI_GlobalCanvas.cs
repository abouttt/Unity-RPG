using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_GlobalCanvas : UI_View
{
    [SerializeField]
    private Image _fadeImage;

    protected override void Init()
    {
        ChangeImageAlpha(_fadeImage, 0f);
        Managers.UI.Register(this);
    }

    public void Fade(float start, float end, float duration)
    {
        ChangeImageAlpha(_fadeImage, start);
        _fadeImage.DOFade(end, duration);
    }

    private void ChangeImageAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
