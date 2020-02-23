using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HR.Logic.BusinessFacade;
using Formula;

namespace HR.Logic.Domain
{
    public partial class S_A_Employee
    {
        /// <summary>
        /// 个人历程(未做完)
        /// </summary>
        /// <returns></returns>
        public List<Course> GetCourse()
        {
            List<Course> listCourse = new List<Course>();
            HREntities entities = FormulaHelper.GetEntities<HREntities>();
            List<S_A_AcademicTitleInformation> listAcademic = entities.Set<S_A_AcademicTitleInformation>().Where(c => c.EmployeeID == this.ID && c.IssueDate != null).ToList();
            foreach (S_A_AcademicTitleInformation item in listAcademic)
            {
                Course course = new Course();
                course.Type = enumCourseType.AcademicTitle;
                course.Date = Convert.ToDateTime(item.IssueDate);
                course.Title = ((enumCourseType)Enum.Parse(typeof(enumCourseType), item.Title)).ToString();
                course.Content = item.Remark;
                listCourse.Add(course);
            }

            List<S_A_WorkPost> listPost = entities.Set<S_A_WorkPost>().Where(c => c.EmployeeID == this.ID && c.EffectiveDate != null).ToList();
            foreach (S_A_WorkPost item in listPost)
            {
                Course course = new Course();
                course.Type = enumCourseType.WorkPost;
                course.Date = Convert.ToDateTime(item.EffectiveDate);
                course.Title = item.Post;
                course.Content = course.Title;
                listCourse.Add(course);
            }

            return listCourse.OrderBy(c => c.Date).ToList();
        }
    }

    public class Course
    {
        public DateTime Date;

        public enumCourseType Type;

        public string Title;

        public string Content;
    }
}
