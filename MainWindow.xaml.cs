using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;
using System.IO;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace Gr2_Sm2_OOP_Lab_13
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private const string ConstFilter = "JSON Files (*.json)|*.json";

		private ObservableCollection<Train> trains;
		private CollectionView trainsView;
		public MainWindow()
		{
			this.trains = new();
			this.trainsView = new(this.trains);

			InitializeComponent();

			this.InfoGrid.ItemsSource = this.trainsView;
			this.NewArrivalDateTimePK.CultureInfo = this.FindArrivalDateTimePK.CultureInfo = this.NewDepartDateTimePK.CultureInfo = new("ru-ru");
		}

		private void AddRecordBT_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				this.trains.Add(new(
					this.NewIdentifierTB.Text.Trim(),
					this.NewDepartPointTB.Text.Trim(),
					this.NewArrivalPointTB.Text.Trim(),
					this.NewDepartDateTimePK.Value.Value,
					this.NewArrivalDateTimePK.Value.Value,
					int.Parse(this.NewNumOfSeatsTB.Text.Trim())));
			}
			catch { return; }
			return;
		}

		private void FindRecordBT_Click(object sender, RoutedEventArgs e)
		{
			string requiredDepartPoint, requiredArrivalPoint;
			DateTime requiredArrivalTimeNotLaterThan;
			try
			{
				requiredDepartPoint = this.FindDepartTB.Text.ToLower().Trim();
				requiredArrivalPoint = this.FindDestTB.Text.ToLower().Trim();
				requiredArrivalTimeNotLaterThan = this.FindArrivalDateTimePK.Value.Value;
			}
			catch { return; }

			var findByDepartAndDest = this.trains.Where(x => x.DeparturePoint.ToLower().Equals(requiredDepartPoint) && x.ArrivalPoint.ToLower().Equals(requiredArrivalPoint)).OrderByDescending(x => x.DepartureTime);
			if (findByDepartAndDest.Count() == 0)
			{
				System.Windows.MessageBox.Show($"Не удалось найти поезда с нужными пунктами отправления и назначения");
				return;
			}

			var findByTime = findByDepartAndDest.FirstOrDefault(x => requiredArrivalTimeNotLaterThan <= x.ArrivalTime);
			if (findByTime is null)
			{
				findByTime = findByDepartAndDest.Last();
				if (findByTime.NumOfFreeSeats <= 0)
				{
					System.Windows.MessageBox.Show($"Не удалось найти поезда с нужными пунктами отправления и назначения и свободными местами");
				}
				else if (
					System.Windows.MessageBox.Show(
						$"Не удалось найти поезд с нужным временем прибытия. Ближайший доступный поезд: {findByTime.TrainIdentifier}, " +
						$"отправление: {findByTime.DepartureTime.ToString()}, прибытие: {findByTime.ArrivalTime.ToString()}. " +
						$"Хотите зарезервировать место?", "Выбор", MessageBoxButton.YesNo)
					.Equals(MessageBoxResult.Yes))
				{
					findByTime.ReserveSeats(1);
				}
			}
			else if (
				System.Windows.MessageBox.Show(
					$"Найден поезд: {findByTime.TrainIdentifier}, " +
					$"отправление: {findByTime.DepartureTime.ToString()}, прибытие: {findByTime.ArrivalTime.ToString()}. " +
					$"Хотите зарезервировать место?", "Выбор", MessageBoxButton.YesNo)
				.Equals(MessageBoxResult.Yes))
			{
				findByTime.ReserveSeats(1);
			}
			this.trainsView.Refresh();
		}

		private void ExportToFileBT_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog Dialog = new SaveFileDialog() { Filter = ConstFilter };
			if (Dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				System.IO.File.WriteAllText(Dialog.FileName, Utf8Json.JsonSerializer.PrettyPrint(Utf8Json.JsonSerializer.Serialize<IEnumerable<Train>>(this.trains.ToList())));
			}
		}

		private void ImportFromFileBT_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog Dialog = new OpenFileDialog() { Filter = ConstFilter, Multiselect = false };

			if (Dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				try
				{
					this.trains.Clear();
					Utf8Json.JsonSerializer.Deserialize<IEnumerable<Train>>(File.ReadAllText(Dialog.FileName)).ToList().ForEach(x=> this.trains.Add(x));
					this.trainsView.Refresh();
				} 
				catch (FormatException exc)
				{
					System.Windows.MessageBox.Show(exc.Message);
				}
			}
		}

		private void DeleteSelectionBT_Click(object sender, RoutedEventArgs e)
		{
			try { this.trains.RemoveAt(this.InfoGrid.SelectedIndex); } catch { return; }
			return;
		}
	}
}
