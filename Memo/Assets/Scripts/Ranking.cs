
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts
{
    public class Ranking
    {
        string path = Application.dataPath + " /ranking.json";
        public Ranking()
        {
        }
        List<Record> Records = new List<Record>();

        public class Record
        {
            public string Date;
            public int MoveNumber;
            public string user ;
        }

        public void setRecord(int moveNumber, string user="Marta")
        {
            Record record = new Record();
            record.Date = DateTime.Today.ToShortDateString();
            record.MoveNumber = moveNumber;
            record.user = user;

            string jsonRecord = JsonUtility.ToJson(record) + Environment.NewLine;
            File.AppendAllText(path, jsonRecord);
        
        }
        private Record readRecord(string record)
        {
            return JsonUtility.FromJson<Record>(record);
        }

       

        public List<Record> getRecords()
        {

            string readText = File.ReadAllText(path);
            String[] records = readText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var rec in records)
            {
                if (rec == "") break;
                Record record = JsonUtility.FromJson<Record>(rec);
                Records.Add(new Record() { Date = record.Date, MoveNumber = record.MoveNumber, user = record.user });
            } 
            return Records;
        }
    }
}
