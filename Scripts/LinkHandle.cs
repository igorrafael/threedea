﻿#if UNITY_EDITOR

using UnityEngine;
using UnityEditor.Experimental.EditorVR;
using UnityEditor.Experimental.EditorVR.Modules;
using UnityEditor.Experimental.EditorVR.UI;
using UnityEditor.Experimental.EditorVR.Utilities;
using UnityEngine.EventSystems;
using Threedea.UI;
using System;

namespace Threedea {
	[RequiresTag(LINK_HANDLE_TAG)]
    internal class LinkHandle : MonoBehaviour, ISelectionFlags, IRayEnterHandler, IRayExitHandler, IRayHoverHandler {
        private const string LINK_HANDLE_TAG = "Link handle";

        private float growFactor = 1.25f;
        private LinkStyler styler;
        [SerializeField]
        private LinkHoverMenu hoverPrefab;
        [SerializeField]
        private LinkHoverMenu hoverInstance;

        public SelectionFlags selectionFlags { get { return m_SelectionFlags; } set { m_SelectionFlags = value; } }
        [SerializeField]
        [FlagsProperty]
        SelectionFlags m_SelectionFlags = SelectionFlags.Ray | SelectionFlags.Direct;

        public void Start() {
            styler = GetComponent<LinkStyler>();
            var go = ObjectUtils.Instantiate(hoverPrefab.gameObject, transform, runInEditMode: true, active: false);
            hoverInstance = go.GetComponent<LinkHoverMenu>();
        }

        internal void PositionBetween(Idea a, Idea b) {
            Vector3 toB = b.transform.position - a.transform.position;
            Vector3 up = Vector3.Lerp(a.transform.up, b.transform.up, 0.5f);
            transform.position = a.transform.position + toB * 0.5f;
            transform.rotation = Quaternion.LookRotation(toB, up);
        }

        public void OnRayEnter(RayEventData eventData) {
            hoverInstance.gameObject.SetActive(true);
        }

        public void OnRayHover(RayEventData eventData) {
            RaycastResult raycastResult = eventData.pointerCurrentRaycast;
            if (raycastResult.gameObject.CompareTag(LINK_HANDLE_TAG)) {
                hoverInstance.transform.position = ClosestPoint(raycastResult.worldPosition);
                hoverInstance.transform.LookAt(CameraUtils.GetMainCamera().transform.position);
                hoverInstance.Dock();
            }
        }

        public void OnRayExit(RayEventData eventData) {
            hoverInstance.gameObject.SetActive(false);
            hoverInstance.Hide();
        }

        /// <summary>
        /// The closest point along the axis of this link
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        private Vector3 ClosestPoint(Vector3 worldPosition) {
            Vector3 toPoint = worldPosition - transform.position;
            Vector3 axis = transform.forward;
            Vector3 offset = axis * Vector3.Dot(toPoint, axis);
            return transform.position + offset;
        }

    }
}
#endif
