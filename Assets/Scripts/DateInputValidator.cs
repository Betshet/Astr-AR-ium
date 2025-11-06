using UnityEngine;
using TMPro;

public class DateInputValidator : MonoBehaviour
{
    [Header("Input Fields")]
    public TMP_InputField dayInput;
    public TMP_InputField monthInput;
    public TMP_InputField yearInput;

    void Start()
    {
        //Validation des saisies ( pour contrôler qu'on rentre pas nimportequoi )
        dayInput.onValueChanged.AddListener(ValidateDay);
        monthInput.onValueChanged.AddListener(ValidateMonth);
        yearInput.onValueChanged.AddListener(ValidateYear);

        // Auto-navigation entre les champs
        dayInput.onValueChanged.AddListener(delegate { CheckAutoNavigation(dayInput, monthInput, 2); });
        monthInput.onValueChanged.AddListener(delegate { CheckAutoNavigation(monthInput, yearInput, 2); });

        //Formater en ajoutant des zéros quand on quitte le champ
        dayInput.onEndEdit.AddListener(FormatDay);
        monthInput.onEndEdit.AddListener(FormatMonth);
        yearInput.onEndEdit.AddListener(FormatYear);
    }

    void ValidateDay(string input)
    {
        if (string.IsNullOrEmpty(input)) return;

        int day = int.Parse(input);

        //On limite entre 1 et 31
        if (day > 31)
        {
            dayInput.text = "31";
        }
        else if (day < 1 && input.Length == 2)
        {
            dayInput.text = "01";
        }
    }

    void ValidateMonth(string input)
    {
        if (string.IsNullOrEmpty(input)) return;

        int month = int.Parse(input);

        //On limite entre 1 et 12
        if (month > 12)
        {
            monthInput.text = "12";
        }
        else if (month < 1 && input.Length == 2)
        {
            monthInput.text = "01";
        }
    }

    void ValidateYear(string input)
    {
        if (string.IsNullOrEmpty(input)) return;
        if (input.Length < 4) return;

        int year = int.Parse(input);

        if (year < 0)
        {
            yearInput.text = "0000";
        }
    }

    //Formater les saisies quand on a fini de remplir
    void FormatDay(string input)
    {
        if (string.IsNullOrEmpty(input)) return;

        int day = int.Parse(input);
        //On formate pour 2 chiffres
        dayInput.text = day.ToString("D2");
    }

    void FormatMonth(string input)
    {
        if (string.IsNullOrEmpty(input)) return;

        int month = int.Parse(input);
        monthInput.text = month.ToString("D2");
    }

    void FormatYear(string input)
    {
        if (string.IsNullOrEmpty(input)) return;

        int year = int.Parse(input);
        //On formate pour 4 chiffres
        yearInput.text = year.ToString("D4");
    }

    void CheckAutoNavigation(TMP_InputField current, TMP_InputField next, int maxLength)
    {
        if (current.text.Length >= maxLength)
        {
            next.Select();
        }
    }

    public string GetFormattedDate()
    {
        string day = dayInput.text;
        string month = monthInput.text;
        string year = yearInput.text;

        //On check que tout est bien rempli
        if (string.IsNullOrEmpty(day) || string.IsNullOrEmpty(month) || string.IsNullOrEmpty(year))
        {
            Debug.LogWarning("Tous les champs de date doivent être remplis !");
            return null;
        }

        //On s'assure du bon formatage au cas où l'utilisateur n'a pas quitté le champ par exemple
        if (day.Length == 1) day = "0" + day;
        if (month.Length == 1) month = "0" + month;
        while (year.Length < 4) year = "0" + year;

        //On construit la string XX/XX/XXXX
        string formattedDate = $"{day}/{month}/{year}";

        return formattedDate;
    }

    public void OnClick_ConfirmDate()
    {
        GameManager.Instance.MoveAllPlanetsToDate(GetFormattedDate());
    }
}