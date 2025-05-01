
using CalculatorApp.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using static CalculatorApp.Model.CalculatorModel;

namespace CalculatorApp.ViewModel
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        private readonly CalculatorModel _calculator = new CalculatorModel();


        private string _inputExpression = string.Empty;
        private string _result = "0";
        private string _currentInput = "0";

        private double? _runningTotal = null;
        private string _pendingOperator = null;
        private double _memory = 0;


        public string InputExpression
        {
            get => _inputExpression;
            set { _inputExpression = value; OnPropertyChanged(nameof(InputExpression)); }
        }

        public string Result
        {
            get => _result;
            set
            {
                _result = value;
                OnPropertyChanged(nameof(Result));
                HasError = !(_result.Contains("Error") || _result.Contains("Cannot divide by zero"));
            }
        }

        private ObservableCollection<HistoryItem> _historyItems;
        public ObservableCollection<HistoryItem> HistoryItems
        {
            get => _historyItems;
            set
            {
                _historyItems = value;

                OnPropertyChanged(nameof(IsHistoryEmpty)); // This is CRITICAL
            }
        }

        private bool _isMemoryAvailable;
        public bool IsMemoryAvailable
        {
            get => _isMemoryAvailable;
            set
            {
                _isMemoryAvailable = value;
                OnPropertyChanged(nameof(IsMemoryAvailable));
            }
        }
        private bool _isMemoryPanelVisible;
        public bool IsMemoryPanelVisible
        {
            get => _isMemoryPanelVisible;
            set
            {
                _isMemoryPanelVisible = value;
                OnPropertyChanged(nameof(IsMemoryPanelVisible));
            }
        }

        private bool _hasError;

        public bool HasError
        {
            get => _hasError;
            set
            {
                _hasError = value;
                OnPropertyChanged(nameof(HasError));
            }
        }


        public ICommand ToggleMemoryPanelCommand { get; }
        public ObservableCollection<double> MemoryList { get; } = new ObservableCollection<double>();
        public ICommand DeleteMemoryCommand { get; }



        private void DeleteMemory(double memoryItem)
        {
            if (memoryItem != null)
            {
                // Assuming MemoryList is an ObservableCollection<string> or similar
                MemoryList.Remove(memoryItem);
                OnPropertyChanged(nameof(IsMemoryEmpty));

            }
        }

        //public ObservableCollection<MemoryItem> MemoryItems { get; } = new ObservableCollection<MemoryItem>();

        //public class MemoryItem
        //{
        //    public double Value { get; set; }

        //    public ICommand MCCommand { get; set; }
        //    public ICommand MPlusCommand { get; set; }
        //    public ICommand MMinusCommand { get; set; }

        //    public MemoryItem(double value, CalculatorViewModel parent)
        //    {
        //        Value = value;

        //        MCCommand = new RelayCommand(_ => parent.RemoveMemoryItem(this));
        //        MPlusCommand = new RelayCommand(_ => parent.AddToMemory(this));
        //        MMinusCommand = new RelayCommand(_ => parent.SubtractFromMemory(this));
        //    }
        //}
        //public ICommand DeleteMemoryCommand { get; }

        //private void DeleteMemory(object obj)
        //{
        //    if (obj is double value && MemoryList.Contains(value))
        //    {
        //        MemoryList.Remove(value);
        //        OnPropertyChanged(nameof(IsMemoryEmpty));

        //    }
        //}

        public bool IsMemoryEmpty => MemoryList.Count == 0;

        public bool IsHistoryEmpty => HistoryItems == null || HistoryItems.Count == 0;

        public ICommand ClearMemoryCommand { get; }

        private bool _isHistoryVisible;

        public bool IsHistoryVisible
        {
            get => _isHistoryVisible;
            set
            {
                _isHistoryVisible = value;
                // Notify the UI that the property has changed (assuming you implement INotifyPropertyChanged)
                OnPropertyChanged(nameof(IsHistoryVisible));
            }
        }

        public ICommand HistoryButtonCommand { get; }
        public ICommand ClearHistoryCommand { get; }

        public ICommand ButtonClickCommand { get; }





        public CalculatorViewModel()
        {
            HistoryItems = new ObservableCollection<HistoryItem>();
            ToggleMemoryPanelCommand = new RelayCommand(ToggleMemoryPanel);
            ClearMemoryCommand = new RelayCommand(ClearMemory);
            HistoryButtonCommand = new RelayCommand(ToggleHistoryVisibility);
            ClearHistoryCommand = new RelayCommand(ClearHistory);
            DeleteMemoryCommand = new RelayCommand<double>(DeleteMemory);
            //ToggleMenuCommand = new RelayCommand(ToggleMenu);



            ButtonClickCommand = new RelayCommand(ExecuteButton);
        }

        private void ToggleHistoryVisibility(object parameter)
        {
            IsHistoryVisible = !IsHistoryVisible;  // Toggle visibility

        }
        private void ClearHistory(object obj)
        {
            HistoryItems.Clear();
            OnPropertyChanged(nameof(IsHistoryEmpty));
        }

        private void ClearMemory(object obj)
        {
            //_memory = 0;
            IsMemoryAvailable = false;
            MemoryList.Clear();
            OnPropertyChanged(nameof(IsMemoryEmpty));

        }

        private void ToggleMemoryPanel(object obj)
        {
            _memory = 0;
            IsMemoryPanelVisible = !IsMemoryPanelVisible;

        }

        public void StoreToMemory()
        {

            if (double.TryParse(Result, out double memVal))
            {
                _memory = memVal;
                MemoryList.Insert(0, memVal);
                IsMemoryAvailable = true;

                OnPropertyChanged(nameof(IsMemoryEmpty));


            }
        }
        //public void RemoveMemoryItem(MemoryItem item)
        //{
        //    MemoryItems.Remove(item);
        //    OnPropertyChanged(nameof(IsMemoryEmpty));
        //}

        //public void AddToMemory(MemoryItem item)
        //{
        //    item.Value += double.TryParse(Result, out var val) ? val : 0;
        //    OnPropertyChanged(nameof(MemoryItems));
        //}

        //public void SubtractFromMemory(MemoryItem item)
        //{
        //    item.Value -= double.TryParse(Result, out var val) ? val : 0;
        //    OnPropertyChanged(nameof(MemoryItems));
        //}

        bool equal = false;
        private void ExecuteButton(object parameter)
        {
            HasError = false;
            string input = parameter.ToString();
            try
            {
                switch (input)
                {

                    case "0":
                    case "1":
                    case "2":
                    case "3":
                    case "4":
                    case "5":
                    case "6":
                    case "7":
                    case "8":
                    case "9":
                    case ".":


                        if (equal && _currentInput == "")
                        {
                            equal = false;
                            _currentInput = input;


                        }
                        else if (equal)
                        {
                            InputExpression = "";
                            equal = false;
                            _currentInput = input;


                        }


                        else
                        {
                            _currentInput += input;

                        }

                        Result = _currentInput;
                        break;

                    case "+":
                    case "-":
                    case "×":
                    case "÷":


                        if (_currentInput != "")
                        {
                            double current = double.Parse(_currentInput);

                            if (_runningTotal == null)
                                _runningTotal = current;
                            else if (_pendingOperator != null)
                                _runningTotal = _calculator.Calculate(_runningTotal.Value, current, _pendingOperator);

                            InputExpression = $"{_runningTotal} {input}";
                            Result = _runningTotal.ToString();
                            _currentInput = "";
                        }

                        _pendingOperator = input;
                        break;

                    case "=":

                        if (_runningTotal != null && _pendingOperator != null && !string.IsNullOrEmpty(InputExpression))
                        {
                            double right;
                            if (_currentInput != "")
                            {
                                right = double.Parse(_currentInput);
                            }

                            else
                            {
                                right = double.Parse(Result);
                            }

                            double result = _calculator.Calculate(_runningTotal.Value, right, _pendingOperator);


                            InputExpression = $"{_runningTotal} {_pendingOperator} {right} =";
                            Result = result.ToString();
                            LogOperation(InputExpression, Result);

                            _currentInput = result.ToString();
                            _runningTotal = null;
                            _pendingOperator = null;

                            HistoryItems.Insert(0, new HistoryItem
                            {
                                Expression = $"{InputExpression}",
                                Result = Result
                            });
                            OnPropertyChanged(nameof(IsHistoryEmpty));

                            equal = true;
                        }
                        break;
                    case "sqrt":
                    case "sqr":
                    case "1/x":
                    case "+/-":
                        string target = !string.IsNullOrEmpty(_currentInput) ? _currentInput : Result;

                        if (double.TryParse(target, out double val))
                        {
                            double result = _calculator.UnaryCalculate(val, input);

                            // Set the appropriate label
                            string displayInput = input switch
                            {
                                "sqrt" => $"√({val})",
                                "sqr" => $"sqr({val})",
                                "1/x" => $"1/({val})",
                                "+/-" => $"-({val})",
                                _ => $"({val})"
                            };

                            InputExpression = displayInput;
                            _currentInput = result.ToString();
                            Result = _currentInput;

                            LogOperation(InputExpression, Result);
                        }
                        break;
                    case "C":
                        HasError = true;

                        OnPropertyChanged(nameof(HasError));
                        InputExpression = "";
                        Result = "0";
                        _currentInput = "";
                        _runningTotal = null;
                        _pendingOperator = null;
                        break;
                    case "CE":

                        HasError = true;
                        OnPropertyChanged(nameof(HasError));
                        _currentInput = "";
                        Result = "0";
                        break;
                    case "⌫":
                        if (InputExpression.Length > 0)
                            InputExpression = "";

                        if (_currentInput.Length > 0)
                        {
                            _currentInput = _currentInput.Substring(0, _currentInput.Length - 1);
                            Result = _currentInput == "" ? "0" : _currentInput;
                        }
                        break;
                    case "MS":

                        StoreToMemory();
                        break;

                    case "MC":
                        _memory = 0;

                        IsMemoryAvailable = false;


                        MemoryList.Clear();
                        OnPropertyChanged(nameof(IsMemoryEmpty));
                        break;

                    case "MR":
                        _currentInput = _memory.ToString();
                        Result = _memory.ToString();

                        break;

                    case "M+":

                        if (double.TryParse(Result, out double addVal))
                        {
                            _memory += addVal;

                            if (MemoryList.Count > 0)
                            {
                                MemoryList[0] = _memory; // update latest memory slot
                            }
                            else
                            {
                                MemoryList.Add(_memory);
                            }

                            IsMemoryAvailable = true;
                            OnPropertyChanged(nameof(IsMemoryEmpty));
                        }
                        break;

                    case "M-":

                        if (double.TryParse(Result, out double subVal))
                        {

                            _memory -= subVal;

                            if (MemoryList.Count > 0)
                            {
                                MemoryList[0] = _memory; // update latest memory slot
                            }
                            else
                            {
                                MemoryList.Add(_memory);
                            }

                            IsMemoryAvailable = true;
                            OnPropertyChanged(nameof(IsMemoryEmpty));
                        }
                        break;


                }
            }
            catch (InvalidOperationException ex)
            {
                Result = ex.Message;
                HasError = true;
                LogError($"InvalidOperationException: {_inputExpression} => {ex.Message}");
            }
            catch (DivideByZeroException ex)
            {
                Result = ex.Message;
                HasError = true;
                LogError($"DivideByZeroException: {_inputExpression} => {ex.Message}");
            }
            catch (Exception ex)
            {
                Result = "Error";
                HasError = true;
                LogError($"Exception: {_inputExpression} => {ex.Message}");
            }

        }
        private void LogOperation(string input, string output)
        {
            using (StreamWriter sw = File.AppendText("calculator.log"))
            {
                sw.WriteLine($"[Operation] {DateTime.Now} | Input: {input} | Output: {output}");
            }
        }

        private void LogError(string error)
        {
            using (StreamWriter sw = File.AppendText("calculator.log"))
            {
                sw.WriteLine($"[Error] {DateTime.Now} | {error}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}


