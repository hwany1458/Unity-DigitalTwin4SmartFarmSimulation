using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Xml;
using TMPro;

public class WeatherController : MonoBehaviour
{
    public TMP_Text SkyStatus;
    public TMP_Text TemperatureStatus;
    public TMP_Text HumidityStatus;
    public TMP_Text WSDStatus;

    static HttpClient client = new HttpClient();

    public void Start()
    {
        CallWeather();
    }

    public void CallWeather()
    {
        DateTime currentDateTime = DateTime.Now;
        string date = currentDateTime.ToString("yyyyMMdd");
        string time = currentDateTime.ToString("HH")+"00";

        Debug.Log(date + ", " + time);

        string url = "http://apis.data.go.kr/1360000/VilageFcstInfoService_2.0/getUltraSrtNcst"; // URL
        url += "?ServiceKey="; // Service Key
        url += "&pageNo=1";
        url += "&numOfRows=1000";
        url += "&dataType=XML";
        url += "&base_date=" + date;
        url += "&base_time="+ time;
        url += "&nx=55";
        url += "&ny=127";

        var request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";

        string results = string.Empty;
        HttpWebResponse response;
        using (response = request.GetResponse() as HttpWebResponse)
        {
            StreamReader reader = new StreamReader(response.GetResponseStream());
            results = reader.ReadToEnd();
        }

        Debug.Log("GETIN");
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(results);

        Debug.Log(results);
        Debug.Log(doc);

        XmlNodeList items = doc.SelectNodes("//items/item");

        foreach (XmlNode item in items)
        {

            string category = item.SelectSingleNode("category").InnerText;
            Debug.Log(category);
            if (category == "T1H")
            {
                double obsrValue = double.Parse(item.SelectSingleNode("obsrValue").InnerText);
                TemperatureStatus.text = obsrValue.ToString() + "°C";
                Debug.Log(TemperatureStatus.text);

            }
            else if (category == "REH")
            {
                double obsrValue = double.Parse(item.SelectSingleNode("obsrValue").InnerText);
                HumidityStatus.text = obsrValue.ToString() + "%";
                Debug.Log(HumidityStatus.text);
            }
            else if (category == "SKY")
            {
                double obsrValue = double.Parse(item.SelectSingleNode("obsrValue").InnerText);
                classificationSkyStatus((int)obsrValue);

            }
            else if (category == "WSD")
            {
                double obsrValue = double.Parse(item.SelectSingleNode("obsrValue").InnerText);
                WSDStatus.text = obsrValue.ToString() + "m/s";
                Debug.Log(WSDStatus.text);
            }

        }
    }

    public void classificationSkyStatus(int opcode)
    {
        if (opcode == 1)
            SkyStatus.text = "Sunny";//맑음
        else if (opcode == 3)
            SkyStatus.text = "Cloudy";//구름많음
        else if (opcode == 4)
            SkyStatus.text = "Overcast";//흐림
    }
}
