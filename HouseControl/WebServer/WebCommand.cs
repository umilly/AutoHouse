using System;
using System.Collections.Generic;
using System.Linq;
using Facade;

public class WebCommand
{
    public WebCommandType Type { get;private set; }
    public List<string> Params { get; private set; }
    public Dictionary<string, string> ParamDict { get; private set; }

    public WebCommand(string type, List<string> @params)
    {

        Type = (WebCommandType)Enum.Parse(typeof(WebCommandType),type);
        Params = @params;
    }
    public WebCommand(string type, Dictionary<string,string> @params)
    {

        Type = (WebCommandType)Enum.Parse(typeof(WebCommandType),type,true);
        ParamDict = @params;
    }

    public bool Json
    {
        get
        {
            switch (Type)
            {
                case WebCommandType.GetParams:
                case WebCommandType.GetModes:
                case WebCommandType.SetParam:
                case WebCommandType.SetMode:
                case WebCommandType.SetSensorsValues:
                    return false;
                case WebCommandType.GetModesJson:
                case WebCommandType.GetParamsJson:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public string Execute(IWebServer serv)
    {
        switch (Type)
        {
            case WebCommandType.GetParams:
                return serv.GetClientParams();
            case WebCommandType.GetModes:
                return serv.GetModes();
            case WebCommandType.SetParam:

                if(Params!=null)
                    return serv.SetParameter(int.Parse(Params[0]), Params[1]);
                if (ParamDict != null)
                    return serv.SetParameter(int.Parse(ParamDict["id"]), ParamDict["val"]);
                else
                    throw new ArgumentException($"Параметры команыды{Type}указаны не верно");
                break;
            case WebCommandType.SetMode:
                return serv.SetMode(int.Parse(Params[0]));
            case WebCommandType.GetModesJson:
                return serv.GetModesJson();
            case WebCommandType.GetParamsJson:
                return serv.GetParametersJson();
            case WebCommandType.SetSensorsValues:
                var sensorValues = ParamDict.Where(a => a.Key != "ip").ToDictionary(pair => pair.Key,pair => pair.Value);
                return serv.SetSensorsValues(ParamDict["ip"], sensorValues);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}