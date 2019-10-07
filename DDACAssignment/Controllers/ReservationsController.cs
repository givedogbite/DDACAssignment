using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DDACAssignment.Data;
using DDACAssignment.Models;
using System.Data.SqlClient;
using System.Data;

namespace DDACAssignment.Views
{
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            return View(await _context.Reservation.ToListAsync());
        }

        public IActionResult FullyBooked()
        {
            return View();
        }

        public IActionResult Successful()
        {
            return View();
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .FirstOrDefaultAsync(m => m.reservation_id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("reservation_id,room_id,Id,check_in,check_out,total_price")] Reservation reservation)
        {
            string availability = "";
            string unavailable = "Not Available";
            int roomid = 0;
            string custid = "";
            // to get empty room id for selected type of room
            using (SqlConnection con = new SqlConnection("Server=tcp:hotel-management.database.windows.net,1433;Initial Catalog=aspnet-DDACAssignment-3DBE6A94-C4E2-4946-B051-DF723269B8F9;Persist Security Info=False;User ID={your_username};Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
            {
                con.Open();

                string username = HttpContext.User.Identity.Name;
                SqlCommand user_id = new SqlCommand("select Id from AspNetUsers where UserName='" + username + "'", con);
                using (SqlDataReader reader = user_id.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        custid = reader.GetString(0);
                        System.Diagnostics.Debug.WriteLine(custid);
                    }
                }
                System.Diagnostics.Debug.WriteLine(custid);
                SqlDataAdapter da = new SqlDataAdapter("select * from Room where room_type ='2 Single Bed'", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count != 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        availability = dt.Rows[i][2].ToString();
                        if (availability == "Available")
                        {
                            roomid = Convert.ToInt32(dt.Rows[i][0]);
                            break;
                        }

                    }
                    if (availability == "Available")
                    {
                        DateTime checkindate = reservation.check_in.Date;
                        DateTime checkoutdate = reservation.check_out.Date;

                        TimeSpan days = checkoutdate - checkindate;
                        int total_days = days.Days;
                        int price = 88;

                        decimal new_price = (total_days) * price;

                        string query2 = "update Room set room_availability ='" + unavailable + "' where room_id ='" + roomid + "'";
                        SqlCommand cmd2 = new SqlCommand(query2, con);
                        cmd2.ExecuteNonQuery();

                        reservation.room_id = roomid;
                        reservation.Id = custid;
                        reservation.check_in = checkindate;
                        reservation.check_out = checkoutdate;
                        reservation.total_price = new_price;
                        if (ModelState.IsValid)
                        {
                            _context.Add(reservation);
                            await _context.SaveChangesAsync();
                            return RedirectToAction(nameof(Successful));
                            //Response.Redirect("Successful.cshtml");
                        }
                    }
                    else
                    {
                        return RedirectToAction(nameof(FullyBooked));
                    }
                }
                con.Close();
            }
            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult CreateQueen()
        {
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateQueen([Bind("reservation_id,room_id,Id,check_in,check_out,total_price")] Reservation reservation)
        {
            string availability = "";
            string unavailable = "Not Available";
            int roomid = 0;
            string custid = "";
            // to get empty room id for selected type of room
            using (SqlConnection con = new SqlConnection("Server=tcp:hotel-management.database.windows.net,1433;Initial Catalog=aspnet-DDACAssignment-3DBE6A94-C4E2-4946-B051-DF723269B8F9;Persist Security Info=False;User ID={your_username};Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
            {
                con.Open();

                string username = HttpContext.User.Identity.Name;
                SqlCommand user_id = new SqlCommand("select Id from AspNetUsers where UserName='" + username + "'", con);
                using (SqlDataReader reader = user_id.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        custid = reader.GetString(0);
                        System.Diagnostics.Debug.WriteLine(custid);
                    }
                }
                System.Diagnostics.Debug.WriteLine(custid);
                SqlDataAdapter da = new SqlDataAdapter("select * from Room where room_type ='1 Queen Bed'", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count != 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        availability = dt.Rows[i][2].ToString();                        
                        if (availability == "Available")
                        {

                            roomid = Convert.ToInt32(dt.Rows[i][0]);

                            DateTime checkindate = reservation.check_in.Date;
                            DateTime checkoutdate = reservation.check_out.Date;

                            TimeSpan days = checkoutdate - checkindate;
                            int total_days = days.Days;
                            int price = 168;

                            decimal new_price = (total_days) * price;

                            string query2 = "update Room set room_availability ='" + unavailable + "' where room_id ='" + roomid + "'";
                            SqlCommand cmd2 = new SqlCommand(query2, con);
                            cmd2.ExecuteNonQuery();

                            reservation.room_id = roomid;
                            reservation.Id = custid;
                            reservation.check_in = checkindate;
                            reservation.check_out = checkoutdate;
                            reservation.total_price = new_price;
                            if (ModelState.IsValid)
                            {
                                _context.Add(reservation);
                                await _context.SaveChangesAsync();
                                return RedirectToAction(nameof(Successful));
                                //Response.Redirect("Successful.cshtml");
                            }
                            break;
                        }
                        else
                        {
                            return RedirectToAction(nameof(FullyBooked));
                        }
                    }
                    
                }
                con.Close();
            }
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("reservation_id,room_id,Id,check_in,check_out,total_price")] Reservation reservation)
        {
            if (id != reservation.reservation_id)
            {
                return NotFound();
            }

            using (SqlConnection con = new SqlConnection("Server=tcp:hotel-management.database.windows.net,1433;Initial Catalog=aspnet-DDACAssignment-3DBE6A94-C4E2-4946-B051-DF723269B8F9;Persist Security Info=False;User ID={your_username};Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
            {
                string custid;
                int price;
                con.Open();

                string username = HttpContext.User.Identity.Name;
                SqlCommand user_id = new SqlCommand("select Id from AspNetUsers where UserName='" + username + "'", con);
                using (SqlDataReader reader = user_id.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        custid = reader.GetString(0);
                    }
                }

                SqlCommand da = new SqlCommand("select room_id, Id from Reservation where reservation_id ='" + id + "'", con);
                using (SqlDataReader reader = da.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int room_id = reader.GetInt32(0);
                        string cust_id = reader.GetString(1);
                        reader.Close();
                        SqlCommand da1 = new SqlCommand("select room_type from Room where room_id ='" + room_id + "'", con);
                        using (SqlDataReader reader1 = da1.ExecuteReader())
                        {
                            while (reader1.Read())
                            {
                                string roomtype = reader1.GetString(0);
                                System.Diagnostics.Debug.WriteLine(roomtype);
                                if (roomtype == "1 Queen Bed")
                                {
                                    System.Diagnostics.Debug.WriteLine("1 queen bed");
                                    price = 128;
                                    DateTime checkindate = reservation.check_in.Date;
                                    DateTime checkoutdate = reservation.check_out.Date;

                                    TimeSpan days = checkoutdate - checkindate;
                                    int total_days = days.Days;

                                    decimal new_price = (total_days) * price;

                                    reservation.room_id = room_id;
                                    reservation.Id = cust_id;
                                    reservation.check_in = checkindate;
                                    reservation.check_out = checkoutdate;
                                    reservation.total_price = new_price;
                                    if (ModelState.IsValid)
                                    {
                                        try
                                        {
                                            _context.Update(reservation);
                                            await _context.SaveChangesAsync();
                                        }
                                        catch (DbUpdateConcurrencyException)
                                        {
                                            if (!ReservationExists(reservation.reservation_id))
                                            {
                                                return NotFound();
                                            }
                                            else
                                            {
                                                throw;
                                            }
                                        }
                                        return RedirectToAction(nameof(Index));
                                    }
                                }
                                else if (roomtype == "2 Single Bed")
                                {
                                    System.Diagnostics.Debug.WriteLine("2 single bed");
                                    price = 88;
                                    DateTime checkindate = reservation.check_in.Date;
                                    DateTime checkoutdate = reservation.check_out.Date;

                                    TimeSpan days = checkoutdate - checkindate;
                                    int total_days = days.Days;

                                    decimal new_price = (total_days) * price;

                                    reservation.room_id = room_id;
                                    reservation.Id = cust_id;
                                    reservation.check_in = checkindate;
                                    reservation.check_out = checkoutdate;
                                    reservation.total_price = new_price;
                                    if (ModelState.IsValid)
                                    {
                                        try
                                        {
                                            _context.Update(reservation);
                                            await _context.SaveChangesAsync();
                                        }
                                        catch (DbUpdateConcurrencyException)
                                        {
                                            if (!ReservationExists(reservation.reservation_id))
                                            {
                                                return NotFound();
                                            }
                                            else
                                            {
                                                throw;
                                            }
                                        }
                                        return RedirectToAction(nameof(Index));
                                    }
                                }
                            }
                        }
                    }
                }
                con.Close();
            }
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .FirstOrDefaultAsync(m => m.reservation_id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int room_id = 0;
            using (SqlConnection con = new SqlConnection("Server=tcp:hotel-management.database.windows.net,1433;Initial Catalog=aspnet-DDACAssignment-3DBE6A94-C4E2-4946-B051-DF723269B8F9;Persist Security Info=False;User ID={your_username};Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
            {
                con.Open();

                SqlCommand da = new SqlCommand("select room_id from Reservation where reservation_id ='" + id + "'", con);
                using (SqlDataReader reader = da.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        room_id = reader.GetInt32(0);
                    }
                    reader.Close();

                    string query = "update Room set room_availability ='Available' where room_id ='" + room_id + "'";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.ExecuteNonQuery();
                }
            }

            var reservation = await _context.Reservation.FindAsync(id);
            _context.Reservation.Remove(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservation.Any(e => e.reservation_id == id);
        }
    }
}
