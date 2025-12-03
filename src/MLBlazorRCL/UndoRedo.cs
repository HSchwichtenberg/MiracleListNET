using System;
using System.Collections.Generic;

namespace MLBlazorRCL;

public class UndoRedoAction
{
 public int ID { get; set; }
 public string Name { get; set; }
 public DateTime DateTime { get; set; }

 public Action DoAction { get; set; }
 public Action UndoAction { get; set; }
}

public class UndoRedoManager
{
 private Stack<UndoRedoAction> _undoStack { get; set; } = new();
 private Stack<UndoRedoAction> _redoStack { get; set; } = new();

 public bool CanUndo => _undoStack.Count > 0;
 public bool CanRedo => _redoStack.Count > 0;

 public string NextUndoName => CanUndo ? _undoStack.Peek().Name : "-";
 public string NextRedoName => CanRedo ? _redoStack.Peek().Name : "-";

 public void Undo()
 {
  if (!CanUndo) return;
  var a = _undoStack.Pop();
  a.UndoAction();
  _redoStack.Push(a);
 }

 public void Redo()
 {
  if (!CanRedo) return;
  var a = _redoStack.Pop();
  a.DoAction();
  _undoStack.Push(a);
 }

 public void Create(UndoRedoAction command)
 {
  _undoStack.Push(command);
  _redoStack.Clear();
 }

 public void Create(string name, Action doAction, Action undoAction)
 {
  var a = new UndoRedoAction() { Name = name, DoAction = doAction, UndoAction = undoAction };
  _undoStack.Push(a);
  _redoStack.Clear();
 }
}