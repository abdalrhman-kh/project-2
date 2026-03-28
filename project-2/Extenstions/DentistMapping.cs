using project_2.Models;
using project_2.ViewModels;

namespace project_2.Extenstions
{
    public static class DentistMapping
    {
        public static DentistViewModel ToViewModel(this Dentist model)
        {
            return new DentistViewModel
            {
                DentistId = model.Id,
                Name = model.Name,
                Experience = model.Experience,
                Collage = model.Collage,
                PhoneNumber = model.PhoneNumber,
                DentistImage = model.DentistImage
            };
        }
        public static Dentist ToModel(this DentistViewModel viewmodel)
        {
            return new Dentist
            {
                Id = viewmodel.DentistId,
                Name = viewmodel.Name,
                Experience = viewmodel.Experience,
                Collage = viewmodel.Collage,
                PhoneNumber = viewmodel.PhoneNumber,
                DentistImage = viewmodel.DentistImage
            };
        }
        public static List<DentistViewModel> ToViewModelList(this List<Dentist> models)
        {
            return models.Select(x => x.ToViewModel()).ToList();
        }
        public static List<Dentist> ToModelList(this List<DentistViewModel> models)
        {
            return models.Select(x => x.ToModel()).ToList();
        }
    }
}
