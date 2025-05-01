using CalculatorApp.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace CalculatorApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = new CalculatorViewModel();
        //this.StateChanged += Window_StateChanged;

    }

    private bool isMenuOpen = false;

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {

        if (e.NewSize.Width > 800)
        {
            //double marginRight = 150;
            ////double marginLeft = e.NewSize.Width - MyTextBox.Width - marginRight;
            //MyTextBox2.FontSize = 70;
            //// Set the TextBox margin to be 200px away from the right side of the window
            //MyTextBox2.Margin = new Thickness(0, MyTextBox2.Margin.Top, marginRight, MyTextBox2.Margin.Bottom);
            //MyTextBox1.Margin = new Thickness(0, MyTextBox1.Margin.Top, marginRight, MyTextBox1.Margin.Bottom);
            AdjustButtonSize(350, 100);  // Larger button size for larger window

        }
        else if (e.NewSize.Width > 400)
        {
            // Medium screen adjustments
            AdjustButtonSize(100, 70);
        }

        else
        {
            //MyTextBox2.FontSize = 30;
            //MyTextBox1.Margin = new Thickness(0, 0, 10, 0);
            //MyTextBox2.Margin = new Thickness(0, 0, 10, 0);
            //MyGrid.Margin = new Thickness(0, 0, 0, 0);
            AdjustButtonSize(75, 50);  // Smaller button size for smaller window
        }
    }



    private void MenuButton_Click(object sender, RoutedEventArgs e)
    {

        if (!isMenuOpen)
        {
            Sidebar.Visibility = Visibility.Visible;

            StandardLabel.Visibility = Visibility.Hidden;

            Storyboard sb = (Storyboard)this.Resources["SlideInMenu"];
            Storyboard.SetTarget(sb.Children[0], Sidebar); // Just to be safe
            sb.Begin();
        }
        else
        {
            Storyboard sb = (Storyboard)this.Resources["SlideOutMenu"];
            Storyboard.SetTarget(sb.Children[0], Sidebar); // Just to be safe
            Sidebar.Visibility = Visibility.Hidden;
            StandardLabel.Visibility = Visibility.Visible;

            sb.Completed += (s, args) =>
            {
                Sidebar.Visibility = Visibility.Hidden;
            };

            sb.Begin();
        }

        isMenuOpen = !isMenuOpen;
    }
    private void SidebarOverlayClose_Click(object sender, RoutedEventArgs e)
    {
        Sidebar.Visibility = Visibility.Collapsed;
        StandardLabel.Visibility = Visibility.Visible;
    }


    private void Grid_MouseEnter(object sender, MouseEventArgs e)
    {

        // Find the current Grid (the parent of the buttons)
        var grid = sender as Grid;
        var brushConverter = new BrushConverter();
        grid.Background = (Brush)brushConverter.ConvertFromString("#FF343434");
        if (grid != null)
        {
            // Find the buttons within the Grid and make them visible
            var plusButton = grid.FindName("PlusButton") as Button;
            var minusButton = grid.FindName("MinusButton") as Button;
            var deleteButton = grid.FindName("DeleteButton") as Button;

            if (plusButton != null && minusButton != null && deleteButton != null)
            {
                plusButton.Visibility = Visibility.Visible;
                minusButton.Visibility = Visibility.Visible;
                deleteButton.Visibility = Visibility.Visible;
            }
        }
    }

    private void Grid_MouseLeave(object sender, MouseEventArgs e)
    {
        // Find the current Grid (the parent of the buttons)
        var grid = sender as Grid;
        if (grid != null)
        {
            // Find the buttons within the Grid and make them hidden
            var plusButton = grid.FindName("PlusButton") as Button;
            var minusButton = grid.FindName("MinusButton") as Button;
            var deleteButton = grid.FindName("DeleteButton") as Button;

            if (plusButton != null && minusButton != null && deleteButton != null)
            {
                plusButton.Visibility = Visibility.Collapsed;
                minusButton.Visibility = Visibility.Collapsed;
                deleteButton.Visibility = Visibility.Collapsed;
            }
        }
    }

    private void MainGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            this.DragMove();
        var vm = DataContext as CalculatorViewModel;

        if (vm != null && vm.IsMemoryPanelVisible)
        {
            // Check if the click was outside the memory panel
            if (!IsClickInsideElement(MemoryPanel, e))
            {
                vm.IsMemoryPanelVisible = false;
            }
        }
    }

    private bool IsClickInsideElement(FrameworkElement element, MouseButtonEventArgs e)
    {
        Point clickPosition = e.GetPosition(element);
        return clickPosition.X >= 0 && clickPosition.X <= element.ActualWidth &&
               clickPosition.Y >= 0 && clickPosition.Y <= element.ActualHeight;
    }



    private void AdjustButtonSize(double width, double height)
    {
        foreach (UIElement child in MyGrid.Children)  // Replace MyGrid with the actual name of your Grid
        {
            if (child is Button button)
            {
                button.Width = width;
                button.Height = height;
            }
        }
    }

    private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            this.DragMove();
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            this.Activate(); // Ensure the window is focused
            this.DragMove();
        }
    }

    private void Minimize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();

    }

    private void Maximize_Click(object sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.Normal)
        {
            WindowState = WindowState.Maximized;

        }
        else
        {
            WindowState = WindowState.Normal;
        }
    }

    private void AboutButton_Click(object sender, RoutedEventArgs e)
    {
        //    AboutWindow about = new AboutWindow();
        //    about.Owner = this; // sets the owner so it appears centered
        //    about.ShowDialog(); // this shows it as a modal popup
        MainContent.Effect = new BlurEffect { Radius = 6 };

        // Create and show the About window
        AboutWindow aboutWindow = new AboutWindow
        {
            Owner = this // Set the owner so it appears on top
        };
        aboutWindow.ShowDialog();

        // Remove blur when About window is closed
        MainContent.Effect = null;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        this.Focus(); // ensure key capture
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        var vm = DataContext as CalculatorViewModel; // Replace with your actual ViewModel

        if (vm?.ButtonClickCommand == null)
            return;

        string input = null;

        // Number keys
        if (e.Key >= Key.D0 && e.Key <= Key.D9)
            input = (e.Key - Key.D0).ToString();
        else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            input = (e.Key - Key.NumPad0).ToString();

        // Operators and symbols
        else if (e.Key == Key.Add) input = "+";
        else if (e.Key == Key.Subtract) input = "-";
        else if (e.Key == Key.Multiply) input = "×";
        else if (e.Key == Key.Divide) input = "÷";
        else if (e.Key == Key.Enter) input = "=";
        else if (e.Key == Key.Decimal) input = ".";
        else if (e.Key == Key.Back) input = "⌫";
        else if (e.Key == Key.Escape) input = "C";
        else if (e.Key == Key.Delete) input = "C";

        // Execute the command if valid key
        if (!string.IsNullOrEmpty(input) && vm.ButtonClickCommand.CanExecute(input))
        {
            vm.ButtonClickCommand.Execute(input);
            e.Handled = true;
        }
    }




}