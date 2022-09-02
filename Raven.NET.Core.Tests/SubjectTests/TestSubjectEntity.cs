using Raven.NET.Core.Subjects;

namespace Raven.NET.Core.Tests.SubjectTests;

public class TestSubjectEntity : RavenSubject
{
    public string Value { get; set; }
    public bool Changed { get; set; }
}