//?---------------------------------------------------------------
//?Copyright?(c)?Coalition of the Good-Hearted Engineers
// FREE TO USE AS LONG AS SOFTWARE FUNDS ARE DONATED TO THE POOR
//?---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using OtripleS.Web.Api.Brokers.DateTimes;
using OtripleS.Web.Api.Brokers.Loggings;
using OtripleS.Web.Api.Brokers.Storage;
using OtripleS.Web.Api.Models.Students;

namespace OtripleS.Web.Api.Services
{
    public partial class StudentService : IStudentService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;

        public StudentService(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
        }

        public ValueTask<Student> RegisterStudentAsync(Student student) =>
        TryCatch(async () =>
        {
            ValidateStudentOnCreate(student);

            return await this.storageBroker.InsertStudentAsync(student);
        });

        public ValueTask<Student> RetrieveStudentByIdAsync(Guid studentId) =>
        TryCatch(async () =>
        {
            ValidateStudentId(studentId);
            Student storageStudent = await this.storageBroker.SelectStudentByIdAsync(studentId);
            ValidateStorageStudent(storageStudent, studentId);

            return storageStudent;
        });

        public ValueTask<Student> ModifyStudentAsync(Student student) =>
        TryCatch(async () =>
        {
            ValidateStudentOnModify(student);

            Student storageStudent =
                await this.storageBroker.SelectStudentByIdAsync(student.Id);

            DateTimeOffset now = this.dateTimeBroker.GetCurrentDateTime();

            return await this.storageBroker.UpdateStudentAsync(student);
        });

        public async ValueTask<Student> DeleteStudentAsync(Guid studentId)
        {
            ValidateStudentId(studentId);
            Student maybeStudent =
                await this.storageBroker.SelectStudentByIdAsync(studentId);

            ValidateStorageStudent(maybeStudent, studentId);

            return await this.storageBroker.DeleteStudentAsync(maybeStudent);
        }
    }
}