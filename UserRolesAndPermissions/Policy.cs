using eLearningApi.UserRolesAndPermissions.Attributes;

namespace eLearningApi.UserRolesAndPermissions;

public enum Policy
{
    /// <summary>
    /// Indicates that a user can only access resources (subjects/courses/modules) which he/she authored.
    /// Not applicable to <see cref="Role.Admin"/>
    /// </summary>
    OwnerPolicy,

    /// <summary>
    /// Can view, update, and delete his/her own account.
    /// </summary>
    [Permitted(Role.Admin, Role.Instructor, Role.Student)]
    UserPolicyOne,

    /// <summary>
    /// Can view and delete all users.
    /// </summary>
    [Permitted(Role.Admin)]
    UserPolicyTwo,

    /// <summary>
    /// Can view any instructor's profile.
    /// </summary>
    [Permitted(Role.Admin, Role.Instructor, Role.Student)]
    UserPolicyThree,

    /// <summary>
    /// Can view any student's profile.
    /// </summary>
    [Permitted(Role.Admin, Role.Instructor)]
    UserPolicyFour,

    /// <summary>
    /// Can view admin's profile.
    /// </summary>
    [Permitted(Role.Admin)]
    UserPolicyFive,

    /// <summary>
    /// Can view published subjects.
    /// </summary>
    [Permitted(Role.Admin, Role.Instructor, Role.Student)]
    SubjectPolicyOne,

    /// <summary>
    /// Can view created unpublished subjects.
    /// </summary>
    [Permitted(Role.Instructor)]
    SubjectPolicyTwo,

    /// <summary>
    /// Can view any unpublished subjects.
    /// </summary>
    [Permitted(Role.Admin)]
    SubjectPolicyThree,

    /// <summary>
    /// Can create a subject.
    /// </summary>
    [Permitted(Role.Instructor)]
    SubjectPolicyFour,

    /// <summary>
    /// Can update created subjects.
    /// </summary>
    [Permitted(Role.Instructor)]
    SubjectPolicyFive,

    /// <summary>
    /// Can delete created subjects.
    /// </summary>
    [Permitted(Role.Instructor)]
    SubjectPolicySix,

    /// <summary>
    /// Can delete any subjects.
    /// </summary>
    [Permitted(Role.Admin)]
    SubjectPolicySeven,

    /// <summary>
    /// Can view published courses.
    /// </summary>
    [Permitted(Role.Admin, Role.Instructor, Role.Student)]
    CoursePolicyOne,

    /// <summary>
    /// Can view created unpublished courses.
    /// </summary>
    [Permitted(Role.Instructor)]
    CoursePolicyTwo,

    /// <summary>
    /// Can view any unpublished courses.
    /// </summary>
    [Permitted(Role.Admin)]
    CoursePolicyThree,

    /// <summary>
    /// Can create a course.
    /// </summary>
    [Permitted(Role.Instructor)]
    CoursePolicyFour,

    /// <summary>
    /// Can update created course.
    /// </summary>
    [Permitted(Role.Instructor)]
    CoursePolicyFive,

    /// <summary>
    /// Can delete created course.
    /// </summary>
    [Permitted(Role.Instructor)]
    CoursePolicySix,

    /// <summary>
    /// Can delete any course.
    /// </summary>
    [Permitted(Role.Admin)]
    CoursePolicySeven,

    /// <summary>
    /// Can view published modules.
    /// </summary>
    [Permitted(Role.Admin, Role.Instructor, Role.Student)]
    ModulePolicyOne,

    /// <summary>
    /// Can view created unpublished modules.
    /// </summary>
    [Permitted(Role.Instructor)]
    ModulePolicyTwo,

    /// <summary>
    /// Can view any unpublised modules.
    /// </summary>
    [Permitted(Role.Admin)]
    ModulePolicyThree,

    /// <summary>
    /// Can create a module.
    /// </summary>
    [Permitted(Role.Instructor)]
    ModulePolicyFour,

    /// <summary>
    /// Can update created modules.
    /// </summary>
    [Permitted(Role.Instructor)]
    ModulePolicyFive,

    /// <summary>
    /// Can delete created modules.
    /// </summary>
    [Permitted(Role.Instructor)]
    ModulePolicySix,

    /// <summary>
    /// Can delete any module.
    /// </summary>
    [Permitted(Role.Admin)]
    ModulePolicySeven,

    /// <summary>
    /// Can view published module contents.
    /// </summary>
    [Permitted(Role.Admin, Role.Instructor, Role.Student)]
    ContentPolicyOne,

    /// <summary>
    /// Can view created unpublished module contents.
    /// </summary>
    [Permitted(Role.Instructor)]
    ContentPolicyTwo,

    /// <summary>
    /// Can view any unpublished module contents.
    /// </summary>
    [Permitted(Role.Admin)]
    ContentPolicyThree,

    /// <summary>
    /// Can create a module content.
    /// </summary>
    [Permitted(Role.Instructor)]
    ContentPolicyFour,

    /// <summary>
    /// Can update created module contents.
    /// </summary>
    [Permitted(Role.Instructor)]
    ContentPolicyFive,

    /// <summary>
    /// Can delete created module contents.
    /// </summary>
    [Permitted(Role.Instructor)]
    ContentPolicySix,

    /// <summary>
    /// Can delete any module content.
    /// </summary>
    [Permitted(Role.Admin)]
    ContentPolicySeven,

    /// <summary>
    /// Can enroll to any published course.
    /// </summary>
    [Permitted(Role.Student)]
    EnrollmentPolicyOne,

    /// <summary>
    /// Can view list of enrolled courses.
    /// </summary>
    [Permitted(Role.Admin, Role.Student)]
    EnrollmentPolicyTwo,

    /// <summary>
    /// Can view progress of enrollment.
    /// </summary>
    [Permitted(Role.Admin, Role.Student)]
    EnrollmentPolicyThree,

    /// <summary>
    /// Can update enrollments.
    /// </summary>
    [Permitted(Role.Student)]
    EnrollmentPolicyFour,

    /// <summary>
    /// Can delete own enrollment.
    /// </summary>
    [Permitted(Role.Student)]
    EnrollmentPolicyFive,

    /// <summary>
    /// Can delete any enrollment.
    /// </summary>
    [Permitted(Role.Admin)]
    EnrollmentPolicySix,
}
