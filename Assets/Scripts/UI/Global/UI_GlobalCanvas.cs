using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_GlobalCanvas : UI_View
{
    private DataBinder _binder;

    protected override void Init()
    {
        _binder = new(gameObject);
        ChangeImageAlpha(_binder.GetImage("FadeImage"), 0f);
        UIManager.Register(this);
    }

    public void Fade(float start, float end, float duration)
    {
        var fadeImage = _binder.GetImage("FadeImage");
        ChangeImageAlpha(fadeImage, start);
        fadeImage.DOFade(end, duration);
    }

    private void ChangeImageAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
