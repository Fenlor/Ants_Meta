using System;
using System.IO;
using Oculus.Platform;
using UnityEngine;
using Application = UnityEngine.Application;

public class RecordManager : MonoBehaviour
{
    //[System.Serializable]
    //public class User
    //{
    //    public int id;        
    //    public float percentComplete = 0f;
    //    public float scenarioOneTime = 0f;
    //    public int scenarioOneErrors = 0;
    //    public float scenarioTwoTime = 0f;
    //    public int scenarioTwoErrors = 0;
    //    public float scenarioThreeTime = 0f;
    //    public int scenarioThreeErrors = 0;
    //    public float scenarioFourTime = 0f;
    //    public int scenarioFourErrors = 0;
    //    public int currentCoins = 0;
    //}
    [System.Serializable]
    public class UserList
    {
        public User[] users;
    }
    public UserList userList = new UserList();
    public int loggedInUserId = -1;

    string filename = "";

    //use this to test coin counts if not going in with a user. or do we just use user 1 to test?
    public int coinCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        filename = Application.persistentDataPath + "/" + "user_records.csv";        
        ReadCsvFile(filename);
        //CreateNewUser();
        //WriteCSV();

        //FOR TESTING PURPOSES
        //CreateNewUser();
    }
    public void ReadCsvFile(string filePath)
    {
        // Check if the file exists
        if (!File.Exists(filePath))
        {
            Debug.Log($"Error: File not found at '{filePath}'");
            return;
        }
        int lineCount = CountLinesInFile(filePath);
        userList.users = new User[lineCount-1];

        try
        {
            // Use a StreamReader to read the file line by line
            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;
                int lineNumber = 0;
                
                // Read until the end of the file
                while ((line = sr.ReadLine()) != null)
                {
                    Debug.Log($"Line {lineNumber + 1}: {line}");

                    if (lineNumber > 0)
                    {                      

                        // You can further process each line here, e.g., split by comma
                        string[] columns = line.Split(',');

                        // Example: Print each column
                        for (int i = 0; i < columns.Length; i++)
                        {
                            Debug.Log($"  Column {i + 1}: {columns[i].Trim()}");
                            
                        }

                        for (int userIndex = 0; userIndex < lineCount; ++userIndex)
                        {
                            userList.users[userIndex] = new User();
                            userList.users[userIndex].id = int.Parse(columns[4 * (userIndex + 1)]);                            
                            userList.users[userIndex].percentComplete = float.Parse(columns[4 * (userIndex + 1) + 1]);
                            userList.users[userIndex].scenarioOneTime = float.Parse(columns[4 * (userIndex + 1) + 2]);
                            userList.users[userIndex].scenarioOneErrors = int.Parse(columns[4 * (userIndex + 1) + 3]);
                            userList.users[userIndex].scenarioTwoTime = float.Parse(columns[4 * (userIndex + 1) + 4]);
                            userList.users[userIndex].scenarioTwoErrors = int.Parse(columns[4 * (userIndex + 1) + 5]);
                            userList.users[userIndex].scenarioThreeTime = float.Parse(columns[4 * (userIndex + 1) + 6]);
                            userList.users[userIndex].scenarioThreeErrors = int.Parse(columns[4 * (userIndex + 1) + 7]);
                            userList.users[userIndex].scenarioFourTime = float.Parse(columns[4 * (userIndex + 1) + 8]);
                            userList.users[userIndex].scenarioFourErrors = int.Parse(columns[4 * (userIndex + 1) + 9]);
                            userList.users[userIndex].currentCoins = int.Parse(columns[4 * (userIndex + 1) + 10]);
                        }

                        Debug.Log("userList.users length:" + userList.users.Length);
                    }

                    lineNumber++;
                }
            }
            Debug.Log("\nCSV file successfully read.");
        }
        catch (Exception ex)
        {
            Debug.Log($"An error occurred: {ex.Message}");
        }
    }

    private int CountLinesInFile(string filePath)
    {
        int counter = 0;
        string line;

        // Read the file and display it line by line.
        System.IO.StreamReader file =
           new System.IO.StreamReader(filePath);
        while ((line = file.ReadLine()) != null)
        {
            Console.WriteLine(line);
            counter++;
        }

        file.Close();
        
        return counter;
    }

    public void WriteCSV()
    {
        Debug.Log("WriteCSV, userList.users.Length:" + userList.users.Length);
        //get user list from record reader
        if (userList.users.Length > 0)
        {
            TextWriter tw = new StreamWriter(filename, false);
            tw.WriteLine("ID, PercentComplete, ScenarioOneTime, ScenarioOneErrors, " +
                "ScenarioTwoTime, ScenarioTwoErrors, ScenarioThreeTime, ScenarioThreeErrors, ScenarioFourTime, ScenarioFourErrors");
            tw.Close();

            tw = new StreamWriter(filename, true);

            for (int userIndex = 0; userIndex < userList.users.Length; ++userIndex)
            {
                tw.WriteLine(userList.users[userIndex].id + "," + userList.users[userIndex].percentComplete + "," +
                    userList.users[userIndex].scenarioOneTime + "," + userList.users[userIndex].scenarioOneErrors +
                    userList.users[userIndex].scenarioTwoTime + "," + userList.users[userIndex].scenarioTwoErrors +
                    userList.users[userIndex].scenarioThreeTime + "," + userList.users[userIndex].scenarioThreeErrors +
                    userList.users[userIndex].scenarioFourTime + "," + userList.users[userIndex].scenarioFourErrors +
                    userList.users[userIndex].currentCoins);
            }
            tw.Close();
        }
    }
    public void CreateNewUser()
    {
        int usersLength = userList.users.Length;
        int newId = usersLength + 1;
        Array.Resize(ref userList.users, userList.users.Length + 1);
        userList.users[usersLength] = new User();
        userList.users[usersLength].id = newId;
        loggedInUserId = newId;
        Debug.Log("CreateNewUser - newId: " + newId);
    }
    public bool AttemptLogin(int userId)
    {
        for (int userIndex = 0; userIndex < userList.users.Length; ++userIndex)
        {
            if(userList.users[userIndex].id == userId)
            {
                loggedInUserId = userId;
                return true;
            }
        }
        return false;
    }


    public User GetLoggedInUser()
    {
        if(loggedInUserId >= 0)
        {
            return userList.users[loggedInUserId-1];
        }
        return null;
    }

    public void AddCoinValue(int coinValue)
    {
        //User loggedInUser = GetLoggedInUser();
        //if(loggedInUser != null)
        //{
        //    userList.users[loggedInUserId].currentCoins += coinValue;
        //    WriteCSV();
        //}

        coinCount += coinValue;
    }

    public int GetCoinValue() 
    {
        //int currentValue = 0;

        //User loggedInUser = GetLoggedInUser();
        //if (loggedInUser != null)
        //{
        //    currentValue = userList.users[loggedInUserId].currentCoins;           
        //}

        //return currentValue;

        return coinCount;
    }
}
