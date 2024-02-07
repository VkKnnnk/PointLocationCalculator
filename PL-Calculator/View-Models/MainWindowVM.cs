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
using System.Windows.Media;
using System.Windows.Shapes;

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
        private ObservableCollection<Line> _tracingLines;
        public ObservableCollection<Line> TracingLines
        {
            get { return _tracingLines; }
            set { _tracingLines = value; }
        }
        #endregion
        public MainWindowVM()
        {
            PolygonPoints = new();
            TracingLines = new();
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
                //Проверяем с разнымыми направлениями вектора, т.к.
                //Есть шанс, что мы попадем вектором в точку пересечения граней
                //Таким образом, мы исключаем такую возможность
                if (PolygonPoints.Count < 3)
                    MessageBox.Show("Точка снаружи", "Сообщение");
                else
                {
                    Point testPoint = new(double.Parse(XTestPointInput), double.Parse(YTestPointInput));
                    TracingLines.Clear();
                    TracingLines.Add(new Line
                    {
                        X1 = testPoint.X,
                        Y1 = testPoint.Y,
                        X2 = 1000,
                        Y2 = 1000
                    });
                    TracingLines.Add(new Line
                    {
                        X1 = testPoint.X,
                        Y1 = testPoint.Y,
                        X2 = 0,
                        Y2 = 0
                    });
                    TracingLines.Add(new Line
                    {
                        X1 = testPoint.X,
                        Y1 = testPoint.Y,
                        X2 = 1000,
                        Y2 = 0
                    });
                    TracingLines.Add(new Line
                    {
                        X1 = testPoint.X,
                        Y1 = testPoint.Y,
                        X2 = 0,
                        Y2 = 1000
                    });
                    OnPropertyChanged("TracingLines");
                    if (CheckLocationPoint())
                        MessageBox.Show("Точка внутри", "Сообщение");
                    else
                        MessageBox.Show("Точка снаружи", "Сообщение");
                }
            }, (param) =>
            {
                if (IsCoordinatesValid(XTestPointInput, YTestPointInput))
                    return true;
                return false;
            });

        #endregion
        #region Methods
        private bool CheckLocationPoint()
        {
            ////////////////////////////////////////////////////////////////////
            ////////////////////////////Что имеем///////////////////////////////
            ////////////////////////////////////////////////////////////////////
            //Если точка:
            //Снаружи - 2 пересечения, либо 0.
            //Внутри - всегда 1 пересечение.

            //Нужен список граней.
            //Пробежаться по списку и сравнить грань с вектором идущим от точки.
            //Координаты точки пересечения не могут быть меньше введенной точки,
            //И должны входить в диапозон точек грани.

            ////////////////////////////////////////////////////////////////////
            ///////////////////////Нахождение `k` и `b`/////////////////////////
            ////////////////////////////////////////////////////////////////////
            //Коэфциент `k` можно найти при условии x1 != x2
            //Имеется линейная функция: y = kx + b,
            //`y` и `x` нам известны, необходимо найти угол наклона `k`
            //и ординату точки пересечения `b`
            //Решается с помощью системы уравнений, т.к. нам известны две точки:
            //kx1 + b - y1 = 0
            //kx2 + b - y2 = 0

            //b = y1 - kx1
            //b = y2 - kx2
            //т.к. оба выражения равны `b`, то:
            //y1 - kx1 = y2 - kx2
            //-kx1 + kx2 = y2 - y1
            //kx2 - kx1 = y2 - y1
            //k(x2 - x1) = y2 - y1
            //k = (y2 - y1)/(x2-x1)

            ////////////////////////////////////////////////////////////////////
            ///////////////////Нахождение точки пересечения/////////////////////
            ////////////////////////////////////////////////////////////////////
            //Далее, чтобы найти точки пересечения двух прямых, нужно решить
            //Систему уравнений двух прямых:
            //y = k1x + b1
            //y = k2x + b2
            //т.к. оба выражения равны `y`, то:
            //k1x + b1 = k2x + b2
            //k1x - k2x = b2 - b1
            //x(k1 - k2) = b2 - b1
            //x = (b2 - b1)/(k1 - k2)

            int amountInside = 0;
            int amountOutside = 0;
            foreach (Line tracingLine in TracingLines)
            {
                double tracingLineMonotonyX = tracingLine.X2 - tracingLine.X1;
                double tracingLineMonotonyY = tracingLine.Y2 - tracingLine.Y1;

                double kTracingLine = (tracingLine.Y2 - tracingLine.Y1) / (tracingLine.X2 - tracingLine.X1);
                //Округляем значение, потому что координатная плоскость смещается по целым числам
                double bTracingLine = Math.Round(tracingLine.Y1 - kTracingLine * tracingLine.X1);

                int amountCrossings = 0;

                for (int i = 0; i < PolygonPoints.Count; i++)
                {
                    Line edge = new();
                    if (i != PolygonPoints.Count - 1)
                    {
                        edge = new Line
                        {
                            X1 = PolygonPoints[i].X,
                            Y1 = PolygonPoints[i].Y,
                            X2 = PolygonPoints[i + 1].X,
                            Y2 = PolygonPoints[i + 1].Y,
                        };
                    }
                    else
                    {
                        edge = new Line
                        {
                            X1 = PolygonPoints[i].X,
                            Y1 = PolygonPoints[i].Y,
                            X2 = PolygonPoints[0].X,
                            Y2 = PolygonPoints[0].Y,
                        };
                    }

                    double xCrossing = 0;
                    double yCrossing = 0;
                    if (edge.X1 != edge.X2)
                    {
                        double kEdge = (edge.Y2 - edge.Y1) / (edge.X2 - edge.X1);
                        double bEdge = edge.Y1 - kEdge * edge.X1;

                        if (kEdge != kTracingLine)
                        {
                            xCrossing = (bTracingLine - bEdge) / (kEdge - kTracingLine);
                            yCrossing = kTracingLine * xCrossing + bTracingLine;
                        }
                    }
                    else
                    {
                        //Нам известно:
                        //x = edge.X1
                        //k2 = kTracingLine
                        //b2 = bTracingLine
                        //Можем найти:
                        //y = k1x + b1
                        xCrossing = edge.X1;
                        yCrossing = kTracingLine * xCrossing + bTracingLine;
                    }
                    bool isCrossingPossibleWithTestPoint = false;
                    //График растет по `x` и `y`
                    if (tracingLineMonotonyX >= 0 && tracingLineMonotonyY >= 0)
                    {
                        if (xCrossing > tracingLine.X1 && yCrossing > tracingLine.Y1)
                            isCrossingPossibleWithTestPoint = true;
                    }
                    //График убывает по `x` и `y`
                    else if (tracingLineMonotonyX <= 0 && tracingLineMonotonyY <= 0)
                    {
                        if (xCrossing < tracingLine.X1 && yCrossing < tracingLine.Y1)
                            isCrossingPossibleWithTestPoint = true;
                    }
                    //График растет по `x` и убывает по `y`
                    else if (tracingLineMonotonyX >= 0 && tracingLineMonotonyY <= 0)
                    {
                        if (xCrossing > tracingLine.X1 && yCrossing < tracingLine.Y1)
                            isCrossingPossibleWithTestPoint = true;
                    }
                    //График растет по `y` и убывает по `x`
                    else if (tracingLineMonotonyX <= 0 && tracingLineMonotonyY >= 0)
                    {
                        if (xCrossing < tracingLine.X1 && yCrossing > tracingLine.Y1)
                            isCrossingPossibleWithTestPoint = true;
                    }
                    if (isCrossingPossibleWithTestPoint)
                    {
                        if ((edge.X1 >= xCrossing && xCrossing >= edge.X2 &&
                            edge.Y1 >= yCrossing && yCrossing >= edge.Y2) ||
                            (edge.X1 <= xCrossing && xCrossing <= edge.X2 &&
                            edge.Y1 <= yCrossing && yCrossing <= edge.Y2) ||
                            (edge.X1 >= xCrossing && xCrossing >= edge.X2 &&
                            edge.Y1 <= yCrossing && yCrossing <= edge.Y2) ||
                            (edge.X1 <= xCrossing && xCrossing <= edge.X2 &&
                            edge.Y1 >= yCrossing && yCrossing >= edge.Y2))
                            amountCrossings++;
                    }
                }
                //Снаружи: 2 пересечения либо 0
                //Внутри: всегда 1 пересечение
                if (amountCrossings % 2 == 0)
                    amountOutside++;
                else
                    amountInside++;
            }
            if (amountOutside > amountInside)
                return false;
            else
                return true;
        }
        private static bool IsCoordinatesValid(string x, string y)
        {
            if (x != String.Empty && y != String.Empty)
                if (double.TryParse(x, out _) && double.TryParse(y, out _))
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
