using UnityEngine;
using UnityEngine.UIElements;

public class DiaryScrollView : MonoBehaviour
{
    public UIDocument uIDocument;
    private int numberOfDiary = 0;
    private int scrollHeight = 850;
    private int scrollWidth = 470;
    Label[] labels;

    private void OnEnable()
    {
        var diaryScrollView = new ScrollView(ScrollViewMode.Vertical);
        diaryScrollView.style.height = scrollHeight;
        diaryScrollView.style.width = scrollWidth;

        labels = new Label[numberOfDiary];

        if (numberOfDiary == 0)
            diaryScrollView.Add(new Label("No diary for now!"));

        for (int i = 0; i < numberOfDiary; i++)
        {
            var label = new Label("JUST MOCK FOR NOW");
            labels[i] = label;
            diaryScrollView.Add(label);
        }
        uIDocument.rootVisualElement.Add(diaryScrollView);
    }


    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
