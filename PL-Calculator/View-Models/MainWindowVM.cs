using PL_Calculator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PL_Calculator.View_Models
{
    public class MainWindowVM : INotifyPropertyChanged
    {
        #region Properties
        private string _xPolygonPointInput;
        public string XPolygonPointInput
        {
            get { return _xPolygonPointInput; }
            set
            {
                _xPolygonPointInput = value;
                OnPropertyChanged("XPolygonPointInput");
            }
        }
        private string _yPolygonPointInput;
        public string YPolygonPointInput
        {
            get { return _yPolygonPointInput; }
            set
            {
                _yPolygonPointInput = value;
                OnPropertyChanged("YPolygonPointInput");
            }
        }
        private string _xTestPointInput;
        public string XTestPointInput
        {
            get { return _xTestPointInput; }
            set
            {
                _xTestPointInput = value;
                OnPropertyChanged("XTestPointInput");
            }
        }
        private string _yTestPointInput;
        public string YTestPointInput
        {
            get { return _yTestPointInput; }
            set
            {
                _yTestPointInput = value;
                OnPropertyChanged("YTestPointInput");
            }
        }
        private ObservableCollection<Point> _polygonPoints;
        public ObservableCollection<Point> PolygonPoints
        {
            get { return _polygonPoints; }
            set { _polygonPoints = value; }
        }
        #endregion
        public MainWindowVM()
        {
            PolygonPoints = new();
        }
        #region Commands
        private ICommand _clearCanvasCommand;
        public ICommand ClearCanvasCommand =>
            _clearCanvasCommand ??= new RelayCommand((param) =>
            {
                PolygonPoints.Clear();
                OnPropertyChanged("PolygonPoints");
            }, (param) =>
             {
                 if (PolygonPoints.Count == 0)
                     return false;
                 return true;
             });
        private ICommand _addPolygonPointCommand;
        public ICommand AddPolygonPointCommand =>
            _addPolygonPointCommand ??= new RelayCommand((param) =>
            {
                Point inputPoint = new(double.Parse(XPolygonPointInput), double.Parse(YPolygonPointInput));
                PolygonPoints.Add(inputPoint);
                OnPropertyChanged("PolygonPoints");
            }, (param) =>
                IsCoordinatesValid(XPolygonPointInput, YPolygonPointInput));
        private ICommand _deletePolygonPointCommand;
        public ICommand DeletePolygonPointCommand =>
            _deletePolygonPointCommand ??= new RelayCommand((param) =>
            {
                PolygonPoints.Remove(PolygonPoints.LastOrDefault());
                OnPropertyChanged("PolygonPoints");
            }, (param) =>
            {
                if (PolygonPoints.Count == 0)
                    return false;
                return true;
            });
        private ICommand _checkLocationStatus;
        public ICommand CheckLocationStatus =>
            _checkLocationStatus ??= new RelayCommand((param) =>
            {

            }, (param) =>
            {
                if (IsCoordinatesValid(XTestPointInput, YTestPointInput) &&
                    PolygonPoints.Count > 2)
                    return true;
                return false;
            });

        #endregion
        #region Methods
        private static bool IsCoordinatesValid(string x, string y)
        {
            if (!String.IsNullOrEmpty(x) && !String.IsNullOrEmpty(y))
                if (double.TryParse(x, out double rezX) && double.TryParse(y, out double rezY))
                    if (rezX > 0 && rezY > 0)
                        return true;
            return false;
        }
        #endregion
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
