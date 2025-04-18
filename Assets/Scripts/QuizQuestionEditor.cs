using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(QuizManager))]
public class QuizQuestionEditor : Editor
{
    private SerializedProperty questionsList;
    private SerializedProperty currentQuestionIndex;
    private SerializedProperty delayBetweenQuestions;
    private SerializedProperty questionText;
    private SerializedProperty answerButtons;
    private SerializedProperty answerTexts;
    private SerializedProperty scoreText;
    private SerializedProperty feedbackText;
    private SerializedProperty questionPanel;
    private SerializedProperty resultPanel;
    private SerializedProperty finalScoreText;
    private SerializedProperty quizScoreText;
    private SerializedProperty totalScoreText;
    private SerializedProperty defaultButtonColor;
    private SerializedProperty correctButtonColor;
    private SerializedProperty incorrectButtonColor;
    
    private SerializedProperty totalQuizTime;
    private SerializedProperty timerText;
    private SerializedProperty timerSlider; // Image yerine Slider
    private SerializedProperty normalTimerColor;
    private SerializedProperty warningTimerColor;
    private SerializedProperty criticalTimerColor;
    private SerializedProperty warningThreshold;
    private SerializedProperty criticalThreshold;
    
    private bool showQuizSettings = true;
    private bool showUIReferences = true;
    private bool showButtonColors = true;
    private bool showTimerSettings = true; // Timer ayarları için yeni bölüm

    private void OnEnable()
    {
        // Find all serialized properties
        questionsList = serializedObject.FindProperty("questions");
        currentQuestionIndex = serializedObject.FindProperty("currentQuestionIndex");
        delayBetweenQuestions = serializedObject.FindProperty("delayBetweenQuestions");
        questionText = serializedObject.FindProperty("questionText");
        answerButtons = serializedObject.FindProperty("answerButtons");
        answerTexts = serializedObject.FindProperty("answerTexts");
        scoreText = serializedObject.FindProperty("scoreText");
        feedbackText = serializedObject.FindProperty("feedbackText");
        questionPanel = serializedObject.FindProperty("questionPanel");
        resultPanel = serializedObject.FindProperty("resultPanel");
        finalScoreText = serializedObject.FindProperty("finalScoreText");
        quizScoreText = serializedObject.FindProperty("quizScoreText");
        totalScoreText = serializedObject.FindProperty("totalScoreText");
        defaultButtonColor = serializedObject.FindProperty("defaultButtonColor");
        correctButtonColor = serializedObject.FindProperty("correctButtonColor");
        incorrectButtonColor = serializedObject.FindProperty("incorrectButtonColor");
        
        totalQuizTime = serializedObject.FindProperty("totalQuizTime");
        timerText = serializedObject.FindProperty("timerText");
        timerSlider = serializedObject.FindProperty("timerSlider"); // Image yerine Slider
        normalTimerColor = serializedObject.FindProperty("normalTimerColor");
        warningTimerColor = serializedObject.FindProperty("warningTimerColor");
        criticalTimerColor = serializedObject.FindProperty("criticalTimerColor");
        warningThreshold = serializedObject.FindProperty("warningThreshold");
        criticalThreshold = serializedObject.FindProperty("criticalThreshold");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        // Quiz Settings
        showQuizSettings = EditorGUILayout.Foldout(showQuizSettings, "Quiz Settings", true);
        if (showQuizSettings)
        {
            EditorGUI.indentLevel++;
            
            EditorGUILayout.PropertyField(delayBetweenQuestions);
            EditorGUILayout.PropertyField(currentQuestionIndex);
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Questions", EditorStyles.boldLabel);
            
            // Questions List
            for (int i = 0; i < questionsList.arraySize; i++)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                SerializedProperty question = questionsList.GetArrayElementAtIndex(i);
                SerializedProperty questionTextProp = question.FindPropertyRelative("questionText");
                SerializedProperty answerOptionsProp = question.FindPropertyRelative("answerOptions");
                SerializedProperty correctAnswerIndexProp = question.FindPropertyRelative("correctAnswerIndex");
                SerializedProperty pointsForCorrectProp = question.FindPropertyRelative("pointsForCorrect");
                SerializedProperty pointsForIncorrectProp = question.FindPropertyRelative("pointsForIncorrect");
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Question {i + 1}", EditorStyles.boldLabel);
                
                if (GUILayout.Button("Remove", GUILayout.Width(80)))
                {
                    questionsList.DeleteArrayElementAtIndex(i);
                    serializedObject.ApplyModifiedProperties();
                    return;
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.PropertyField(questionTextProp, new GUIContent("Question"));
                
                EditorGUILayout.LabelField("Answer Options:");
                for (int j = 0; j < 4; j++)
                {
                    if (j < answerOptionsProp.arraySize)
                    {
                        SerializedProperty answerOption = answerOptionsProp.GetArrayElementAtIndex(j);
                        EditorGUILayout.BeginHorizontal();
                        
                        // Mark correct answer with color
                        bool isCorrect = j == correctAnswerIndexProp.intValue;
                        Color originalColor = GUI.color;
                        if (isCorrect)
                        {
                            GUI.color = Color.green;
                        }
                        
                        EditorGUILayout.PropertyField(answerOption, new GUIContent($"Option {j + 1}"));
                        
                        if (isCorrect)
                        {
                            GUILayout.Label("✓", EditorStyles.boldLabel, GUILayout.Width(20));
                        }
                        else if (GUILayout.Button("Set as Correct", GUILayout.Width(120)))
                        {
                            correctAnswerIndexProp.intValue = j;
                        }
                        
                        GUI.color = originalColor;
                        EditorGUILayout.EndHorizontal();
                    }
                }
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(pointsForCorrectProp, new GUIContent("Points for Correct"));
                EditorGUILayout.PropertyField(pointsForIncorrectProp, new GUIContent("Points for Incorrect"));
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(10);
            }
            
            // Add new question button
            if (GUILayout.Button("Add New Question"))
            {
                questionsList.arraySize++;
                SerializedProperty newQuestion = questionsList.GetArrayElementAtIndex(questionsList.arraySize - 1);
                
                // Initialize new question
                newQuestion.FindPropertyRelative("questionText").stringValue = "Enter question text here";
                SerializedProperty answerOptions = newQuestion.FindPropertyRelative("answerOptions");
                answerOptions.arraySize = 4; // Ensure 4 answer options
                
                for (int i = 0; i < 4; i++)
                {
                    answerOptions.GetArrayElementAtIndex(i).stringValue = $"Answer option {i + 1}";
                }
                
                newQuestion.FindPropertyRelative("correctAnswerIndex").intValue = 0;
                newQuestion.FindPropertyRelative("pointsForCorrect").intValue = 10;
                newQuestion.FindPropertyRelative("pointsForIncorrect").intValue = -5;
            }
            
            EditorGUI.indentLevel--;
        }
        
        // Timer Settings - Yeni bölüm
        EditorGUILayout.Space(10);
        showTimerSettings = EditorGUILayout.Foldout(showTimerSettings, "Timer Settings", true);
        if (showTimerSettings)
        {
            EditorGUI.indentLevel++;
            
            // Ana timer özellikleri
            EditorGUILayout.PropertyField(totalQuizTime, new GUIContent("Total Quiz Time (seconds)"));
            EditorGUILayout.PropertyField(timerText, new GUIContent("Timer Text"));
            EditorGUILayout.PropertyField(timerSlider, new GUIContent("Timer Slider"));
            
            // Timer renkleri
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Timer Colors", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(normalTimerColor, new GUIContent("Normal Color"));
            EditorGUILayout.PropertyField(warningTimerColor, new GUIContent("Warning Color"));
            EditorGUILayout.PropertyField(criticalTimerColor, new GUIContent("Critical Color"));
            
            // Timer eşikleri
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Timer Thresholds", EditorStyles.boldLabel);
            EditorGUILayout.Slider(warningThreshold, 0f, 1f, new GUIContent("Warning Threshold"));
            EditorGUILayout.Slider(criticalThreshold, 0f, 1f, new GUIContent("Critical Threshold"));
            
            // Yardımcı metin
            EditorGUILayout.HelpBox("Warning Threshold: When timer reaches this percentage, it turns yellow.\nCritical Threshold: When timer reaches this percentage, it turns red.", MessageType.Info);
            
            EditorGUI.indentLevel--;
        }
        
        // UI References
        EditorGUILayout.Space(10);
        showUIReferences = EditorGUILayout.Foldout(showUIReferences, "UI References", true);
        if (showUIReferences)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(questionText);
            EditorGUILayout.PropertyField(answerButtons, true);
            EditorGUILayout.PropertyField(answerTexts, true);
            EditorGUILayout.PropertyField(scoreText);
            EditorGUILayout.PropertyField(feedbackText);
            EditorGUILayout.PropertyField(questionPanel);
            EditorGUILayout.PropertyField(resultPanel);
            EditorGUILayout.PropertyField(finalScoreText);
            EditorGUILayout.PropertyField(quizScoreText);
            EditorGUILayout.PropertyField(totalScoreText);
            EditorGUI.indentLevel--;
        }
        
        // Button Colors
        EditorGUILayout.Space(10);
        showButtonColors = EditorGUILayout.Foldout(showButtonColors, "Button Colors", true);
        if (showButtonColors)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(defaultButtonColor);
            EditorGUILayout.PropertyField(correctButtonColor);
            EditorGUILayout.PropertyField(incorrectButtonColor);
            EditorGUI.indentLevel--;
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}
#endif