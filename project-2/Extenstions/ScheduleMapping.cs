using project_2.Models;
using project_2.ViewModels;

namespace project_2.Extenstions
{
    public static class ScheduleMapping
    {
        public static ScheduleViewModel ToViewModel(this Schedule model)
        {
            return new ScheduleViewModel
            {
                ScheduleId = model.Id,
                Date = model.Date,
                StartTime = TimeOnly.FromTimeSpan(model.StartTime),
                EndTime = TimeOnly.FromTimeSpan(model.EndTime),
                Status = model.Status,
            };
        }
        public static Schedule ToModel(this ScheduleViewModel viewmodel)
        {
            return new Schedule
            {
                Id = viewmodel.ScheduleId,
                Date = viewmodel.Date!.Value,
                StartTime = viewmodel.StartTime!.Value.ToTimeSpan(),
                EndTime = viewmodel.EndTime!.Value.ToTimeSpan(),
                Status = viewmodel.Status,
            };
        }
        public static List<ScheduleViewModel> ToViewModelList(this List<Schedule> models)
        {
            return models.Select(x => x.ToViewModel()).ToList();
        }
        public static List<Schedule> ToModelList(this List<ScheduleViewModel> models)
        {
            return models.Select(x => x.ToModel()).ToList();
        }
    }
}
