namespace project_2.Models
{
    public class Appointment:BaseEntity
    {
        public int ScheduleId { get; set; }
        public Schedule Schedule { get; set; }

        public String Subject { get; set; }
        public String PatientName { get; set; }
        public String PhoneNumber {  get; set; }
        public String Email { get; set; }
        public TimeSpan SlotStartTime { get; set; }
        public bool Status { get; set; } = true;

    }
}
