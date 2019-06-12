using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using System;
using System.Globalization;

public class SqliteControler
{
    private static string databaseName = "Memo_database";
    public string connectionPath = "URI=file:" + Application.persistentDataPath + "/" + databaseName;
    public SqliteControler()
    {
        // Create database
        Debug.Log(connectionPath);

        //tworzenie tabel
        CreateUserTable();
        CreateGameTable();
        
    }
    // Start is called before the first frame update
    // Use this for initialization
    void Start()
    {

        // Create database
        /*   string connection = "URI=file:" + Application.persistentDataPath + "/" + databaseName;
           Debug.Log(connection);
           IDbConnection dbcon = OpenDb(connection);

           //tworzenie tabel
           CreateUserTable();
           CreateGameTable();

           //dodawanie rekordow
           InsertUser("Jola");                                          //zwraca bool, trzeba sprawdzić czy użytkownik został dodany
           InsertGame(dbcon, 10, 10, "owoce", "6", "02.06.2019", "Jola");

           //dodawanie rekordow
           InsertUser(dbcon, "Marta");
           InsertGame(dbcon, 15, 10, "warzywa", "6", "03.06.2019", "Marta");     //zwraca bool, trzeba sprawdzić czy gra została dodana - jak nie ma takiego uzytkonika to nie doda

           //przyklady jak czytac z tabel
           ReadFromUserTable(dbcon);
           ReadFromGameTable(dbcon);
           */
        // Close connection
       // dbcon.Close();

    }

    public IDbConnection OpenDb(string connection)
    {
        // Open connection
        IDbConnection dbcon = new SqliteConnection(connection);
        dbcon.Open();
        return dbcon;
    }

    public string ReadFromGameTable(string userName)
    {
        string recordsString = "";
        IDbConnection dbcon = OpenDb(connectionPath);
        IDbCommand cmnd_readGame = dbcon.CreateCommand();

        IDataReader readerGame;
        readerGame = queryToDatabase(userName, cmnd_readGame);
        recordsString += TakeResultsFromDatabase( readerGame);

        /*     IDataReader readerGame1;
             IDataReader readerGame2;
             IDataReader readerGame3;

             string category = "owoce";
             readerGame3 = queryToDatabase(userName, cmnd_readGame, category);
             recordsString  += TakeResultsFromCategory(category, readerGame3);

             category = "warzywa";
             readerGame1 = queryToDatabase(userName, cmnd_readGame, category);
             recordsString += TakeResultsFromCategory(category, readerGame1);

             category = "kroliczki";
             readerGame2 = queryToDatabase(userName, cmnd_readGame, category);
             recordsString += TakeResultsFromCategory(category, readerGame2);
             */
        dbcon.Close();
        return recordsString;
    }
    private static string TakeResultsFromDatabase( IDataReader readerGame)
    {
        string recordsString = "";
        recordsString += MenuBehavior.playerName + "\n";
        recordsString += " ------------------------- \n";
        int index = 1;
        while (readerGame.Read())
        {
            recordsString += index + ". " +
                             readerGame[5].ToString() + " " +
                             readerGame[2].ToString() + " s, " +
                             "kategoria: " + readerGame[3].ToString() + ", "+
                             "level: " + readerGame[4].ToString() + ".\n";

            Debug.Log(recordsString);
            index++;
        }

        return recordsString;
    }
        private static string TakeResultsFromCategory(string category, IDataReader readerGame)
    {
        string recordsString = "";
        recordsString += category + "\n";
        recordsString += " ------------------------- \n";
        while (readerGame.Read())
        {
            recordsString += readerGame[0].ToString() + ". " +
                             readerGame[5].ToString() + " " +
                             readerGame[2].ToString() + " s, " +
                             "level: " + readerGame[4].ToString() + ".\n ";

            Debug.Log(recordsString);
        }

        return recordsString;
    }

    private static IDataReader queryToDatabase(string userName, IDbCommand cmnd_readGame)
    {
        IDataReader readerGame;
        string queryGame = $"SELECT * " +
             $"FROM Game " +
             $"WHERE Game.userName = \"{userName}\"" +
             $"ORDER BY time " +
             $"LIMIT 8 ";
        cmnd_readGame.CommandText = queryGame;
        readerGame = cmnd_readGame.ExecuteReader();
        return readerGame;
    }
    private static IDataReader queryToDatabaseWithCategory(string userName, IDbCommand cmnd_readGame, string category)
    {
        IDataReader readerGame;
        string queryGame = $"SELECT * " +
             $"FROM Game " +
             $"WHERE Game.userName = \"{userName}\"" +
             $"AND Game.category = \"{category}\"" +
             $"ORDER BY time " +
             $"LIMIT 3 ";
        cmnd_readGame.CommandText = queryGame;
        readerGame = cmnd_readGame.ExecuteReader();
        return readerGame;
    }

    public void ReadFromUserTable()
    {
        // Read and print all values in table
        IDbConnection dbcon = OpenDb(connectionPath);
        IDbCommand cmnd_read = dbcon.CreateCommand();
        IDataReader reader;
        string query = $"SELECT * FROM User";
        cmnd_read.CommandText = query;
        reader = cmnd_read.ExecuteReader();

        while (reader.Read())
        {
            Debug.Log("name: " + reader[0].ToString());
        }
        dbcon.Close();
    }

    public bool InsertGame( int moveNumber, double time, string category, int level, string date, string userName)
    {
        IDbConnection dbcon = OpenDb(connectionPath);

      
         // Insert values in table Game
        IDbCommand cmndGame = dbcon.CreateCommand();
        cmndGame.CommandText = $"INSERT INTO Game (moveNumber, time, category, level, date, userName) " +
                                  $"VALUES" + $" ({moveNumber},\"{time.ToString()}\",\"{category}\",{level}, \"{date}\",\"{userName}\")";
        cmndGame.ExecuteNonQuery();
        Debug.Log($"Game for {userName} added.");

        dbcon.Close();
        return true;
    }

    private void CreateUserTable()
    {
        IDbConnection dbcon = OpenDb(connectionPath);

        // Create table User
        IDbCommand dbcmd = dbcon.CreateCommand();
        string q_createTableUser = " CREATE TABLE IF NOT EXISTS User (name TEXT NOT NULL PRIMARY KEY UNIQUE )";

        dbcmd.CommandText = q_createTableUser;
        dbcmd.ExecuteReader();
        dbcon.Close();
    }

    private void CreateGameTable()
    {
           IDbConnection dbcon = OpenDb(connectionPath);
        // Create table Game
        IDbCommand dbcmdGame = dbcon.CreateCommand();
        string q_createTableGame = " CREATE TABLE IF NOT EXISTS Game " +
            "(id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
            "moveNumber INTEGER NOT NULL," +
            "time TEXT NOT NULL," +
            "category TEXT NOT NULL," +
            "level INTEGER NOT NULL," +
            "date TEXT NOT NULL," +
            "userName TEXT NOT NULL, FOREIGN KEY(\"userName\") REFERENCES \"User\"(\"name\") ON DELETE CASCADE)";

        dbcmdGame.CommandText = q_createTableGame;
        dbcmdGame.ExecuteReader();
        dbcon.Close();
    }

    public bool InsertUser(string name)
    {
        IDbConnection dbcon = OpenDb(connectionPath);
        //sprawdzanie czy  juz takie uzytkownika -> jak tak to nie wpisuje
        if (IsUserExistInDatabase(name))
        {
            dbcon.Close();
            return false;
        }
      
        // Insert values in table User
        IDbCommand cmnd = dbcon.CreateCommand();
        cmnd.CommandText = $"INSERT INTO User (name) VALUES (\"{name}\")";
        Debug.Log($"User {name} added.");
        cmnd.ExecuteNonQuery();
        dbcon.Close();
        return true;
    }

    private bool IsUserExistInDatabase( string name)
    {
        IDbConnection dbcon = OpenDb(connectionPath);
        IDbCommand cmnd_read = dbcon.CreateCommand();
        IDataReader reader;
        string query = $"SELECT * FROM User Where User.name = \"{name}\"";
        cmnd_read.CommandText = query;
        reader = cmnd_read.ExecuteReader();
        if (reader.Read())
        {
            dbcon.Close();
            return true;
        };

        dbcon.Close();
        return false;
    }

    // Update is called once per frame
    void Update()
    {

    }

}

public class User
{
   public string name;
}

public class Game
{
    int id;
    int moveNumber;
    string time;
    string category;
    int level;
    DateTime date;
    string userName;
}
