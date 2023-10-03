using System.Collections.Generic;
using UnityEngine;

namespace Game.Script
{
    public class SphereMoveUndo : MonoBehaviour
    {
        #region Fields

        private List<SphereAddForce> _commandList = new();

        #endregion

        #region Properties

        public int CommandListCount => _commandList.Count;

        #endregion
        
        #region Public Methods

        public void AddCommand(ICommand command)
        {
            _commandList.Add(command as SphereAddForce);
            command.Do();
        }

        public void UndoCommand()
        {
            if (_commandList.Count == 0) return;

            var command = _commandList[^1];
            command.Undo();
            _commandList.Remove(command);
        }

        #endregion
    }
}
