using SIRGA.Web.ViewComponents;

namespace SIRGA.Web.Helpers
{
    public static class FormFieldsHelper
    {
        private const string UserIconPath = "M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z";
        private const string EmailIconPath = "M16 12a4 4 0 10-8 0 4 4 0 008 0zm0 0v1.5a2.5 2.5 0 005 0V12a9 9 0 10-9 9m4.5-1.206a8.959 8.959 0 01-4.5 1.207";
        private const string PhoneIconPath = "M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z";
        private const string LocationIconPath = "M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z";
        private const string AddressIconPath = "M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6";
        private const string SectorIconPath = "M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4";
        private const string DocumentIconPath = "M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z";

        public static ValidatedFormFieldViewModel FirstNameField(string colorScheme = "blue")
        {
            return new ValidatedFormFieldViewModel
            {
                PropertyName = "FirstName",
                Label = "Nombre",
                IconPath = UserIconPath,
                ColorScheme = colorScheme,
                IsRequired = true
            };
        }

        public static ValidatedFormFieldViewModel LastNameField(string colorScheme = "blue")
        {
            return new ValidatedFormFieldViewModel
            {
                PropertyName = "LastName",
                Label = "Apellido",
                IconPath = UserIconPath,
                ColorScheme = colorScheme,
                IsRequired = true
            };
        }

        public static ValidatedFormFieldViewModel EmailField(string colorScheme = "blue")
        {
            return new ValidatedFormFieldViewModel
            {
                PropertyName = "Email",
                Label = "Correo Electrónico",
                InputType = "email",
                IconPath = EmailIconPath,
                ColorScheme = colorScheme
            };
        }

        public static ValidatedFormFieldViewModel PhoneField(string colorScheme = "blue")
        {
            return new ValidatedFormFieldViewModel
            {
                PropertyName = "PhoneNumber",
                Label = "Teléfono",
                IconPath = PhoneIconPath,
                Placeholder = "(809) 000-0000",
                ColorScheme = colorScheme
            };
        }

        public static ValidatedFormFieldViewModel ProvinceField(string colorScheme = "blue")
        {
            return new ValidatedFormFieldViewModel
            {
                PropertyName = "Province",
                Label = "Provincia",
                IconPath = LocationIconPath,
                ColorScheme = colorScheme
            };
        }

        public static ValidatedFormFieldViewModel SectorField(string colorScheme = "blue")
        {
            return new ValidatedFormFieldViewModel
            {
                PropertyName = "Sector",
                Label = "Sector",
                IconPath = SectorIconPath,
                ColorScheme = colorScheme
            };
        }

        public static ValidatedFormFieldViewModel AddressField(string colorScheme = "blue")
        {
            return new ValidatedFormFieldViewModel
            {
                PropertyName = "Address",
                Label = "Dirección",
                InputType = "textarea",
                IconPath = AddressIconPath,
                ColorScheme = colorScheme,
                TextAreaRows = 3
            };
        }

        public static ValidatedFormFieldViewModel SpecialtyField(string colorScheme = "purple")
        {
            return new ValidatedFormFieldViewModel
            {
                PropertyName = "Specialty",
                Label = "Especialidad",
                InputType = "select",
                IconPath = DocumentIconPath,
                ColorScheme = colorScheme,
                Options = new List<SelectOption>
                {
                    new SelectOption("", "Seleccione una especialidad..."),
                    new SelectOption("Matemáticas", "Matemáticas"),
                    new SelectOption("Edu. Física", "Edu. Física"),
                    new SelectOption("Lengua Española", "Lengua Española"),
                    new SelectOption("Informática", "Informática"),
                    new SelectOption("Ciencias Sociales", "Ciencias Sociales"),
                    new SelectOption("Ciencias Naturales", "Ciencias Naturales"),
                    new SelectOption("Educación Artística", "Educación Artística")
                }
            };
        }
    }
}
