using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gridnine.FlightCodingTest
{
    class Program
    {
        static void Main(string[] args)
        {
            FlightBuilder flightBuilder = new FlightBuilder();
            var flights = flightBuilder.GetFlights();
            List<Filter> filters = new List<Filter> { new TimeFilter(), new SegmentFilter(), new GroundFilter() };
            Print(flights, filters, flights);
            while (true)
            {
                string input = Console.ReadLine();
                if (input.ToLower() == "exit")
                {
                    break;
                }
                else
                {
                    if (input == "")
                    {
                        Print(flights, filters, flights);
                    }
                    else
                    {
                        if(input.Contains(" "))
                        {
                            string[] nums = input.Split(' ');
                            var tmpFlights = flights;
                            bool flag = false;
                            foreach(var num in nums)
                            {
                                if (num == "")
                                    continue;
                                try
                                {
                                    tmpFlights = ChooseFilter(num, filters, tmpFlights);
                                }
                                catch(Exception)
                                {
                                    Error();
                                    Default(filters);
                                    flag = true;
                                    break;
                                }
                            }
                            if (flag)
                            {
                                continue;
                            }
                            Print(tmpFlights, filters,flights);
                        }
                        else
                        {
                            try
                            {
                                Print(ChooseFilter(input, filters, flights), filters, flights);
                            }
                            catch(Exception)
                            {
                                Error();
                                Default(filters);
                                continue;
                            }
                        }
                    }
                }
            }
        }

        public static void Error()
        {
            Console.Clear();
            string result = "Несуществующий номер фильтра! Поторите ввод:\n\r";
            Console.Write(result);
        }

        public static IList<Flight> ChooseFilter(string num, List<Filter> filters, IList<Flight> flights)
        {
            int number;
            try
            {
                number = int.Parse(num);

                if ((number) >= 1 && (number) < filters.Count + 1)
                {
                    return filters[number - 1].FilterFlights(flights);
                }
                else
                    if (number == 0)
                {
                    return null;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void Print(IList<Flight> flights, List<Filter> filters, IList<Flight> defaultFlights)
        {
            Console.Clear();
            if (flights != null)
            {
                foreach (var flight in flights)
                {
                    string result = $"{flights.IndexOf(flight) + 1}.  Начало полета: {flight.Segments[0].DepartureDate}  Конец полета: {flight.Segments[flight.Segments.Count() - 1].ArrivalDate}\n\r";
                    foreach (var segment in flight.Segments)
                    {
                        result += $"\tСегмент № {flight.Segments.IndexOf(segment) + 1}: Отправление: {segment.DepartureDate} - Прибытие: {segment.ArrivalDate} \n\r";
                    }
                    Console.Write(result);
                }
            }
            else
            {
                foreach(var filter in filters)
                {
                    Print(ChooseFilter((filters.IndexOf(filter) + 1).ToString(), filters,defaultFlights), filters);
                }
            }
            Default(filters);
        }

        public static void Print(IList<Flight> flights, List<Filter> filters)
        {
            Console.Clear();
            foreach (var flight in flights)
            {
                string result = $"{flights.IndexOf(flight) + 1}.  Начало полета: {flight.Segments[0].DepartureDate}  Конец полета: {flight.Segments[flight.Segments.Count() - 1].ArrivalDate}\n\r";
                foreach (var segment in flight.Segments)
                {
                    result += $"\tСегмент № {flight.Segments.IndexOf(segment) + 1}: Отправление: {segment.DepartureDate} - Прибытие: {segment.ArrivalDate} \n\r";
                }
                Console.Write(result);
            }
        }

        public static void Default(List<Filter> filters)
        {
            string result = "Список фильтров:\n\r";
            foreach (var filter in filters)
            {
                result += $"{filters.IndexOf(filter) + 1}. {filter.About()}\n\r";
            }
            result += $"Для применения сразу нескольких фильтров введите номера через пробел\n\r" +
                $"Для вывода по умолчанию (все фильтры применяются по очереди) введите 0\n\r" +
                $"Для вывода изначального списка перелетов нажмите Enter без номера\n\r" +
                $"Для выхода введите Exit\n\r" +
                $"Введите номер(а) фильтров\n\r";
            Console.Write(result);
        }
    }

    abstract class Filter
    {
        public abstract IList<Flight> FilterFlights(IList<Flight> flights);
        public abstract string About();
    }

    class TimeFilter : Filter
    {
        public override string About()
        {
            return "Фильтрует перелеты по вылету до текущего момента времени";
        }

        public override IList<Flight> FilterFlights(IList<Flight> flights)
        {
            IList<Flight> result = new List<Flight>();
            for (int i = 0; i < flights.Count; i++)
            {
                var flight = flights[i];
                bool flag = false;
                foreach (var segment in flight.Segments)
                {
                    if (segment.DepartureDate < DateTime.Now)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    result.Add(flight);
                    continue;
                }
            }
            return result;
        }
    }

    class SegmentFilter : Filter
    {
        public override string About()
        {
            return "Фильтрует перелеты по сегментам с датой прилёта раньше даты вылета";
        }

        public override IList<Flight> FilterFlights(IList<Flight> flights)
        {
            IList<Flight> result = new List<Flight>();
            for (int i = 0; i < flights.Count; i++)
            {
                var flight = flights[i];
                bool flag = false;
                foreach (var segment in flight.Segments)
                {
                    if (segment.DepartureDate > segment.ArrivalDate)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    result.Add(flight);
                    continue;
                }
            }
            return result;
        }
    }

    class GroundFilter : Filter
    {
        public override string About()
        {
            return "Фильтрует перелеты по общему времи, проведённому на земле";
        }

        public override IList<Flight> FilterFlights(IList<Flight> flights)
        {
            IList<Flight> result = new List<Flight>();
            for (int i = 0; i < flights.Count; i++)
            {
                var flight = flights[i];
                bool flag = false;
                int j = 0;
                TimeSpan timeOnGround = new TimeSpan(0,0,0,0);
                if (flight.Segments.Count > 1)
                {
                    while (j<flight.Segments.Count - 1)
                    {
                        timeOnGround += (flight.Segments[j + 1].DepartureDate - flight.Segments[j].ArrivalDate);
                        if(timeOnGround.TotalHours > 2.0)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                if (!flag)
                {
                    result.Add(flight);
                    continue;
                }

            }
            return result;
        }
    }
}

