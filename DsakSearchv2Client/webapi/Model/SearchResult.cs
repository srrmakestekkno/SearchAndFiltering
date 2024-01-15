﻿

using webapi.DTOs;
using webapi.Helpers;

namespace webapi.Model
{
    public struct SearchResult
    {
        public const int MaxItems = 2000;

        public SearchResult(string header)
        {
            Header = header;
            Query = null;
            Tickets = new Dsak[0];
            NumberOfTickets = 0;
        }

        public SearchResult(Dsak[] tickets, Search search)
        {
            if (tickets.Length == 0)
            {
                Header = "Fant ingen d:saker som matchet søket";
            }
            else
            {
                Header = "Fant " + tickets.Length + " d:sak" + (tickets.Length > 1 ? "er" : "") + " som matchet søket";
            }

            if (search.IsTruncated)
            {
                Header += ", søkestrengen ble avkortet";
            }

            Query = SearchParser.ParseSearch(search);
            Tickets = tickets;
            NumberOfTickets = tickets.Length;
        }

        public SearchResult(Dsak[] tickets, Search search, int numberOfTicketsFound)
            : this(tickets, search)
        {
            if (numberOfTicketsFound > tickets.Length)
            {
                Header = "Fant " + numberOfTicketsFound + " d:saker som matchet søket, de " + MaxItems + " første ble returnert";
            }

            NumberOfTickets = numberOfTicketsFound;
        }

        public string Header { get; }
        public string Query { get; }
        public Dsak[] Tickets { get; }
        public IEnumerable<Company> Companies { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public int NumberOfTickets { get; }
        public int NumberOfManagers { get; set; }
        public Dictionary<string, int> UniqueCompanyOccurances { get; set; }
    }
}