using UnityEngine;

namespace Game.Script
{
    public class SphereAddForce : ICommand
    {
        #region Fields

        private Rigidbody2D _rb;
        private Vector3 _direction;
        private float _force;

        #endregion

        #region Constructor

        public SphereAddForce(Rigidbody2D rb, Vector3 direction, float force)
        {
            _rb = rb;
            _direction = direction;
            _force = force;
        }        

        #endregion

        #region Public Methods

        public void Do()
        {
            _rb.AddForce(_direction * _force);
        }

        public void Undo()
        {
            _rb.AddForce(-1 * _direction * _force);
        }

        #endregion
    }
}
