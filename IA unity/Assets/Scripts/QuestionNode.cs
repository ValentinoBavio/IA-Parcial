using System;

public class QuestionNode : INode
{
    private Func<bool> _question;
    private INode _nodeTrue;
    private INode _nodeFalse;

    public QuestionNode(Func<bool> question, INode nodeTrue, INode nodeFalse)
    {
        _question = question;
        _nodeTrue = nodeTrue;
        _nodeFalse = nodeFalse;
    }

    public void Execute()
    {
        if (_question())
        {
            _nodeTrue.Execute();
        }
        else
        {
            _nodeFalse.Execute();
        }
    }
}