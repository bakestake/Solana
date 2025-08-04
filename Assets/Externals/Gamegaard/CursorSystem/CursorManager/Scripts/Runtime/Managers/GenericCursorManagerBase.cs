using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamegaard.CursorSystem
{
    public abstract class GenericCursorManagerBase<TCursorData, TCursorDataType> : CursorManagerBase where TCursorData : CursorData2D<TCursorDataType>
    {
        [SerializeField] private TCursorData defaultCursorData;
        [SerializeField] private CursorInputHandler inputHandler;

        private readonly Dictionary<int, List<TCursorData>> cursorPriorityMap = new Dictionary<int, List<TCursorData>>();
        private readonly Dictionary<TCursorData, HashSet<object>> cursorRequests = new Dictionary<TCursorData, HashSet<object>>();

        public CursorInputHandler InputHandler => inputHandler;
        public TCursorData DefaultCursorData => defaultCursorData;
        public TCursorData ActiveCursor { get; private set; }
        protected string ActiveStateName { get; private set; }
        private float cursorUpdateTime;
        private int frameIndex;

        protected virtual void Reset()
        {
            inputHandler.TryAssignDefaultLeftClickAction();
        }

        protected virtual void Start()
        {
            InputHandler.Initialize();
            InputHandler.OnClick += OnClickInput;
            InputHandler.Enable();

            if (defaultCursorData != null)
            {
                SetCursor(defaultCursorData);
            }
        }

        protected virtual void OnDestroy()
        {
            InputHandler.OnClick -= OnClickInput;
            InputHandler.Disable();
        }

        protected virtual void Update()
        {
            InputHandler.Update();

            if (ActiveCursor == null) return;

            CursorState<TCursorDataType> state = ActiveCursor.GetState(ActiveStateName);

            if (state.IsAnimated && Time.time >= cursorUpdateTime)
            {
                frameIndex = (frameIndex + 1) % state.FrameCount;
                ApplyFrame(state, frameIndex);
                cursorUpdateTime = Time.time + state.AnimationSpeed;
            }
        }

        public override void SetDefaultCursorAsGeneric(BaseCursorData cursorData, int cursorID = 0)
        {
            if (cursorData is TCursorData specificCursorData)
            {
                SetDefaultCursor(specificCursorData);
            }
            else
            {
                Debug.LogWarning("Invalid cursor Type");
            }
        }

        public void SetDefaultCursor(TCursorData cursor)
        {
            defaultCursorData = cursor;
            SetCursor(defaultCursorData);
        }

        private void OnClickInput(bool isClicking)
        {
            if (isClicking && ActiveCursor != null)
            {
                SetCursorState("Click");
            }
            else if (!isClicking && ActiveCursor != null)
            {
                SetCursorState("Default");
            }
        }

        private void AddCursor(TCursorData cursorData)
        {
            if (!cursorPriorityMap.ContainsKey(cursorData.Priority))
            {
                cursorPriorityMap[cursorData.Priority] = new List<TCursorData>();
            }

            if (!cursorPriorityMap[cursorData.Priority].Contains(cursorData))
            {
                cursorPriorityMap[cursorData.Priority].Add(cursorData);
                cursorRequests[cursorData] = new HashSet<object>();
            }
        }

        public override BaseCursorData GetActiveCursorData(int cursorID = 0)
        {
            return ActiveCursor;
        }

        public override BaseCursorData GetDefaultCursorData(int cursorID = 0)
        {
            return defaultCursorData;
        }

        public override void RequestCursorAsGeneric(BaseCursorData cursorData, object requester, string stateName = "Default", int cursorID = 0)
        {
            if (cursorData is TCursorData specificCursorData)
            {
                RequestCursor(specificCursorData, requester, stateName);
            }
            else
            {
                Debug.LogWarning("Invalid cursor Type");
            }
        }

        public override void ReleaseCursorAsGeneric(BaseCursorData cursorData, object requester, int cursorID = 0)
        {
            if (cursorData is TCursorData specificCursorData)
            {
                ReleaseCursor(specificCursorData, requester);
            }
            else
            {
                Debug.LogWarning("Invalid cursor Type");
            }
        }

        public void RequestCursor(TCursorData cursorData, object requester, string stateName = "Default")
        {
            if (!IsValidQurest(cursorData, requester)) return;

            AddCursor(cursorData);

            if (cursorRequests[cursorData].Add(requester))
            {
                SetCursor(cursorData, stateName);
            }
        }

        public override void ReleaseAll(int cursorID = 0)
        {
            foreach (TCursorData cursorData in cursorRequests.Keys.ToList())
            {
                cursorRequests[cursorData].Clear();
                if (cursorPriorityMap.TryGetValue(cursorData.Priority, out var list))
                {
                    list.Remove(cursorData);
                }
            }

            cursorRequests.Clear();
            cursorPriorityMap.Clear();

            SetCursor(defaultCursorData);
        }

        public void ReleaseCursor(TCursorData cursorData, object requester)
        {
            if (!IsValidQurest(cursorData, requester)) return;

            if (cursorRequests.TryGetValue(cursorData, out HashSet<object> requesters) && requesters.Remove(requester))
            {
                if (requesters.Count == 0)
                {
                    cursorPriorityMap[cursorData.Priority].Remove(cursorData);
                    cursorRequests.Remove(cursorData);
                }

                SetHighestPriorityCursor();
            }
        }

        private bool IsValidQurest(TCursorData cursorData, object requester)
        {
            return cursorData != null && requester != null;
        }

        private void SetHighestPriorityCursor()
        {
            foreach (int priority in cursorPriorityMap.Keys)
            {
                foreach (TCursorData cursor in cursorPriorityMap[priority])
                {
                    if (cursorRequests[cursor].Count > 0)
                    {
                        SetCursor(cursor);
                        return;
                    }
                }
            }

            SetCursor(defaultCursorData);
        }

        public override void SetCursorState(string stateName, int cursorID = 0)
        {
            if (ActiveCursor == null) return;

            CursorState<TCursorDataType> state = ActiveCursor.GetState(stateName);
            if (state.Name != ActiveStateName)
            {
                frameIndex = 0;
                ActiveStateName = state.Name;
                ApplyState(state);
            }
        }

        private void SetCursor(TCursorData cursorData, string stateName = "Default")
        {
            if (!EqualityComparer<TCursorData>.Default.Equals(ActiveCursor, cursorData))
            {
                ActiveCursor = cursorData;
                frameIndex = 0;
                ActiveStateName = stateName;
                ApplyState(cursorData.GetState(stateName));
            }
        }

        protected abstract void ApplyState(CursorState<TCursorDataType> state);
        protected abstract void ApplyFrame(CursorState<TCursorDataType> state, int frameIndex);
    }
}