using System;
using System.Collections.Generic;
using System.Text;

namespace Gr2_Sm2_OOP_Lab_13
{
	public class Train
	{
		public string TrainIdentifier { get; private init; }
		public string DeparturePoint { get; private init; }
		public string ArrivalPoint { get; private init; }
		public DateTime DepartureTime { get; private init; }
		public DateTime ArrivalTime { get; private init; }
		//public TimeSpan OnwayTime => this.ArrivalTime - this.DepartureTime;
		public int NumOfTotalSeats { get; private init; }
		public int NumOfSeatsOccupied { get; private set; } = 0;
		public int NumOfFreeSeats => this.NumOfTotalSeats - this.NumOfSeatsOccupied;

		public Train(string trainIdentifier, string departurePoint, string arrivalPoint, DateTime departureTime, DateTime arrivalTime, int numOfTotalSeats)
		{
			this.TrainIdentifier = trainIdentifier;
			this.DeparturePoint = departurePoint;
			this.ArrivalPoint = arrivalPoint;
			this.DepartureTime = departureTime;
			this.ArrivalTime = arrivalTime;
			this.NumOfTotalSeats = numOfTotalSeats;
		}

		public void ReserveSeats(int numOfSeatsToReserve)
		{
			if (numOfSeatsToReserve < 0 || numOfSeatsToReserve > this.NumOfFreeSeats) throw new ArgumentException();

			this.NumOfSeatsOccupied += numOfSeatsToReserve;
		}
		public void FreeupSeats(int numOfSeatsToFreeup)
		{
			if (numOfSeatsToFreeup < 0 || numOfSeatsToFreeup > this.NumOfSeatsOccupied) throw new ArgumentException();

			this.NumOfSeatsOccupied -= numOfSeatsToFreeup;
		}
	}
}
