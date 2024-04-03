using eLearningApi.Models;

namespace eLearningApi.DataAccess;

public class UnitOfWork : IDisposable
{
    private bool _isDisposed = false;
    private readonly AppDbContext _context = default!;
    private BaseRepository<Content> _contents = default!;
    private BaseRepository<Course> _courses = default!;
    private BaseRepository<Enrollment> _enrollments = default!;
    private BaseRepository<EnrollmentModule> _enrollmentModules = default!;
    private BaseRepository<Module> _modules = default!;
    private BaseRepository<Subject> _subjects = default!;
    private BaseRepository<User> _users = default!;
    private TokenRepository _tokens = default!;

    public BaseRepository<Content> Contents => _contents ??= new BaseRepository<Content>(_context);
    public BaseRepository<Course> Courses => _courses ??= new BaseRepository<Course>(_context);
    public BaseRepository<Enrollment> Enrollments => _enrollments ??= new BaseRepository<Enrollment>(_context);
    public BaseRepository<EnrollmentModule> EnrollmentModules => _enrollmentModules ??= new BaseRepository<EnrollmentModule>(_context);
    public BaseRepository<Module> Modules => _modules ??= new BaseRepository<Module>(_context);
    public BaseRepository<Subject> Subjects => _subjects ??= new BaseRepository<Subject>(_context);
    public BaseRepository<User> Users => _users ??= new BaseRepository<User>(_context);
    public TokenRepository Tokens => _tokens ??= new TokenRepository(_context);


    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Dispose(bool disposing)
    {
        if (disposing && !_isDisposed)
        {
            _context.Dispose();
        }

        _isDisposed = true;
    }

    public void Save()
    {
        _context.SaveChanges();
    }
}
