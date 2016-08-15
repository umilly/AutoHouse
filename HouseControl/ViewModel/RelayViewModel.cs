using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Input;
using Facade;
using ViewModel;
using ViewModelBase;

public class RelayViewModel : ViewModelBase.ViewModelBase
{
    private RelayData _relayData;
    private string _isAvailable;

    public bool? State
    {
        get { return _state; }
        set { _state = value; }
    }

    public RelayData RelayData
    {
        get { return _relayData; }
        set
        {
            _relayData = value; 
            OnPropertyChanged("Number");
            OnPropertyChanged("Name");
            OnPropertyChanged("Address");
            OnPropertyChanged("StartCommand");
            OnPropertyChanged("StopCommand");
            OnPropertyChanged("IsAvailable");
        }
    }

    public int Number
    {
        get { return RelayData.Number; }
    }
    private CommandHandler _onOffCommand;
    private bool? _state;

    private  void Action(bool b)
    {
        Switch(b);
    }

    public ICommand OnOffCommand
    {
        get { return _onOffCommand ?? (_onOffCommand = new CommandHandler(Action)); }
    }

    public void Switch(bool on)
    {
        if (SwitchInternal(@on))
            State = on;
        OnPropertyChanged("State");
    }

    private bool SwitchInternal(bool @on)
    {
        if (IsAvailable != "Success")
        {
            if (Use<IPool>().GetViewModels<SettingsVM>().Single().IsDebug)
            {
                Use<IViewService>().ShowMessage("Address not available");
            }
            return false;
        }
        try
        {
            SendCommand(@on ? StartCommand : StopCommand);
            return true;
        }
        catch (Exception e)
        {
            if (Use<IPool>().GetViewModels<SettingsVM>().Single().IsDebug)
            {
                Use<IViewService>().ShowMessage(e.ToString());
            }
            return false;
        }
    }

    private void SendCommand(string command)
    {
        var res = string.Empty;
        var url = "http://" + Address + "/" + command;
            //"http://localhost:3000/status";
        try
        {
            Use<INetworkService>().SyncRequest(url);
            //var request = WebRequest.Create(url);
            //request.Credentials = CredentialCache.DefaultCredentials;
            //var response = request.GetResponse();
            //res = ((HttpWebResponse) response).StatusDescription;
            //var reader = new StreamReader(response.GetResponseStream());
            //res = reader.ReadToEnd();
        }
        finally
        {
            if (Use<IPool>().GetViewModels<SettingsVM>().Single().IsDebug)
            {
                Use<IViewService>().ShowMessage(string.Format("url:{0} \r\n response:\r\n {1}",url,res));
            }
        }
    }

    public string Name
    {
        get { return RelayData .Name; }
        set
        {
            RelayData.Name = value;
        }
    }

    public void UpdateIsAvailable()
    {
        IsAvailable  = Use<INetworkService>().Ping(Address).ToString();
    }

    public string Address
    {
        get { return RelayData .Address; }
        set
        {
            RelayData.Address = value;
            UpdateIsAvailable();
        }
    }

    public double Opacity
    {
        get { return IsAvailable == "Success" ? 1 : 0.5; }
    }
    

    public string StartCommand
    {
        get { return RelayData.StartCommand; }
        set { RelayData.StartCommand = value; }
    }

    public string StopCommand
    {
        get { return RelayData.StopCommand; }
        set { RelayData.StopCommand = value; }
    }

    public string IsAvailable
    {
        get { return _isAvailable; }
        set
        {
            _isAvailable = value; 
            OnPropertyChanged();
            OnPropertyChanged("Opacity");
        }
    }

    public RelayViewModel(IServiceContainer container) : base(container)
    {
    }

    public override int ID
    {
        get { return RelayData.Number; }
        set { }
    }
}