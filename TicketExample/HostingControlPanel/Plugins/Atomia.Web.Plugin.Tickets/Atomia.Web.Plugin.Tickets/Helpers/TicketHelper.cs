using Atomia.Web.Plugin.Example.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atomia.Web.Plugin.Example.Helpers
{
    public static class TicketHelper
    {
        public static void createTickets()
        {
            TicketModel ticket = new TicketModel();
            ticket.Ticketid = "10001";
            ticket.HiddenId = 1;
            ticket.Subject = "Ticket1";
            ticket.Message = "Some random message";
            ticket.Status = "Open";
            ticket.Emergency = true;
            TicketsList.Instance.Add(ticket);

            ticket = new TicketModel();
            ticket.Ticketid = "10002";
            ticket.HiddenId = 2;
            ticket.Subject = "Ticket2";
            ticket.Message = "Some random message";
            ticket.Status = "Open";
            ticket.Emergency = true;
            TicketsList.Instance.Add(ticket);

            ticket = new TicketModel();
            ticket.Ticketid = "10003";
            ticket.HiddenId = 3;
            ticket.Subject = "Ticket3";
            ticket.Message = "Some random message";
            ticket.Status = "Open";
            ticket.Emergency = true;
            TicketsList.Instance.Add(ticket);

            ticket = new TicketModel();
            ticket.Ticketid = "10004";
            ticket.HiddenId = 4;
            ticket.Subject = "Ticket4";
            ticket.Message = "Some random message";
            ticket.Status = "Open";
            ticket.Emergency = true;
            TicketsList.Instance.Add(ticket);

            ticket = new TicketModel();
            ticket.Ticketid = "10005";
            ticket.HiddenId = 5;
            ticket.Subject = "Ticket5";
            ticket.Message = "Some random message";
            ticket.Status = "Open";
            ticket.Emergency = true;
            TicketsList.Instance.Add(ticket);
        }

        public static void createReplies()
        {
            ReplyModel reply = new ReplyModel();
            reply.From = "Andjela";
            reply.HiddenId = 1;
            reply.Attachement = "atch";
            reply.Message = "Random message";
            RepliesList.Instance.Add(reply);

            reply = new ReplyModel();
            reply.From = "Andjela";
            reply.HiddenId = 2;
            reply.Attachement = "atch";
            reply.Message = "Random message";
            RepliesList.Instance.Add(reply);

            reply = new ReplyModel();
            reply.From = "Andjela";
            reply.HiddenId = 3;
            reply.Attachement = "atch";
            reply.Message = "Random message";
            RepliesList.Instance.Add(reply);

            reply = new ReplyModel();
            reply.From = "Andjela";
            reply.HiddenId = 4;
            reply.Attachement = "atch";
            reply.Message = "Random message";
            RepliesList.Instance.Add(reply);
        }

        public static TicketModel getTicket(int id)
        {
            int index = id - 1;
            return TicketsList.Instance[index];
        }

        public static List<TicketModel> searchForResult(List<TicketModel> lista, string sSearch)
        {
            if (String.IsNullOrEmpty(sSearch))
            {
                return TicketsList.Instance;
            }
            else
            {
                foreach (TicketModel el in TicketsList.Instance)
                {
                    if (el.Ticketid.Equals(sSearch) || el.Subject.Equals(sSearch) || el.Date.ToString().Equals(sSearch) || el.Message.Equals(sSearch) || el.Status.Equals(sSearch))
                    {
                        lista.Add(el);
                    }
                }

                return lista;
            }
        }

        public static List<ReplyModel> searchForResultReply(List<ReplyModel> lista, string sSearch)
        {
            if (String.IsNullOrEmpty(sSearch))
            {
                return RepliesList.Instance;
            }
            else
            {
                foreach (ReplyModel el in RepliesList.Instance)
                {
                    if (el.From.Equals(sSearch) || el.Message.Equals(sSearch) || el.Date.ToString().Equals(sSearch))
                    {
                        lista.Add(el);
                    }
                }

                return lista;
            }
        }
    }
}
