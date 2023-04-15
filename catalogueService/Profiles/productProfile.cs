using catalogueService.Database;
using catalogueService.Models;
using catalogueService.requestETresponse;
using AutoMapper;
using catalogueService.Database.DBsets;
using catalogueService.Authentication;
using catalogueService.Models;

namespace catalogueService.Profiles
{
    public class FeeProfile : AutoMapper.Profile
    {
        public FeeProfile()
        {
            CreateMap<Fee, FeeModel>().ReverseMap();
            CreateMap<Fee, FeeRequest>().ReverseMap();
            CreateMap<category, categoryModel>().ReverseMap();
            CreateMap<category, categoryRequest>().ReverseMap();
            CreateMap<customer, customerModel>().ReverseMap();
            CreateMap<customer, customerRequest>().ReverseMap();
            CreateMap<location, locationModel>().ReverseMap();
            CreateMap<location, locationRequest>().ReverseMap();
            CreateMap<users, userModel>().ReverseMap();
            CreateMap<users, userRequest>().ReverseMap();
            CreateMap<users, authUserRequest>().ReverseMap();
            CreateMap<users, authUserModel>().ReverseMap();
            CreateMap<type, typeModel>().ReverseMap();
            CreateMap<type, typeRequest>().ReverseMap();
            CreateMap<orderModel, orders>().ReverseMap();
            CreateMap<orderRequest, orders>().ReverseMap();
            CreateMap<sales, saleModel>().ReverseMap();
            CreateMap<admin, adminModel>().ReverseMap();
            CreateMap<admin, adminRequest>().ReverseMap();
            CreateMap<student, studentModel>().ReverseMap();
            CreateMap<student, studentRequest>().ReverseMap();
            CreateMap<Teacher, teacherModel>().ReverseMap();
            CreateMap<Teacher, teacherRequest>().ReverseMap();
            CreateMap<Exam, ExamModel>().ReverseMap();
            CreateMap<Exam, examRequest>().ReverseMap();
        }
    }
}
