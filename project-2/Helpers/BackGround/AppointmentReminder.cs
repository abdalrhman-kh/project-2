using Microsoft.EntityFrameworkCore;
using project_2.Data;
using project_2.Helpers.Email;
using project_2.Models;
using project_2.Models.Repositories;

namespace project_2.Helpers.BackGround
{
    public class AppointmentReminder : BackgroundService
    {
            private readonly IServiceProvider _serviceprovider;
        public AppointmentReminder(IServiceProvider serviceprovider)
        {
            _serviceprovider = serviceprovider;
        }
        protected override async Task ExecuteAsync(CancellationToken stopToken)
        {
            while (!stopToken.IsCancellationRequested)
            {
                await sendReminder();

                await Task.Delay(TimeSpan.FromMinutes(40), stopToken);
            }
        }
        private async Task sendReminder()
        {
            using var scop = _serviceprovider.CreateScope();
            var dbcontext = scop.ServiceProvider.GetRequiredService<DataDbContext>();
            var emailsender = scop.ServiceProvider.GetRequiredService<IEmailSender>();
            var dentistrepo = scop.ServiceProvider.GetRequiredService<IRepository<Dentist>>();

            var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

            var appointments = await dbcontext.Appointment.AsTracking().Include(x => x.Schedule).Where(x => !x.IsDeleted && !x.ReminderFlag && x.Schedule.Date == tomorrow).ToListAsync();
            if (!appointments.Any()) return;

            var dentist = dentistrepo.GetAll().FirstOrDefault();

            foreach(var apt in appointments)
            {
                try
                {
                    string subject = "Appointment Reminder";
                    string body = $"Hello {apt.PatientName},\n\n" +
                        $"this is a reminder you have an appointment tomorrow. \n\n" +
                        $"Subject: {apt.Subject}.\n" +
                        $"Date: {apt.Schedule.Date}.\n" +
                        $"Time: {apt.SlotStartTime:hh\\:mm}\n\n";

                    await emailsender.SendEmailAsync(apt.Email, subject, body);
                }
                catch
                {

                }
                apt.ReminderFlag = true;
            }

            await dbcontext.SaveChangesAsync();

            if(dentist != null)
            {
                string subject = $"Tomorrows appointments - {tomorrow}";
                string body = $"you have {appointments.Count} appointment(s) tomorrow\n\n";
                foreach (var apt in appointments)
                    body += $"- {apt.SlotStartTime:hh\\:mm} | {apt.PatientName} || {apt.Subject}\n";
                await emailsender.SendEmailAsync(dentist.Email, subject, body);
            }

        }
    }
}
