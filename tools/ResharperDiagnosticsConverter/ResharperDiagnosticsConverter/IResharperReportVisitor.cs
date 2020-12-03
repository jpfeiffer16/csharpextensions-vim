using System;

namespace ResharperDiagnosticsConverter
{
    public interface IResharperReportVisitor
    {
        public void VisitIssues(Action<string, int, int, int, int, string> issueVisitor);
    }
}
