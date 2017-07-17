using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
	private readonly float _turnSpeed = 6.0f;      // Speed of camera turning when mouse moves in along an axis
	private readonly float _panSpeed = 6.0f;       // Speed of the camera when being panned
	private readonly float _zoomSpeed = 6.0f;      // Speed of the camera going back and forth

	private readonly float _keyZoomSpeed = 16.0f;
	private readonly float _keyPanSpeed = 16.0f;

	private Vector3 _mouseOrigin;    // Position of cursor when mouse dragging starts
	private bool _isPanning;     // Is the camera being panned?
	private bool _isRotating;    // Is the camera being rotated?
	private bool _isZooming;     // Is the camera zooming?

	private Ray _ray;
	private RaycastHit _hit;


	void Update()
	{
		// Get the left mouse button
		if (Input.GetMouseButtonDown(0))
		{
			// Get mouse origin
			_mouseOrigin = Input.mousePosition;
			_isRotating = true;
		}

		// Get the right mouse button
		if (Input.GetMouseButtonDown(1))
		{
			// Get mouse origin
			_mouseOrigin = Input.mousePosition;
			_isPanning = true;
		}

		// Get the middle mouse button
		if (Input.GetMouseButtonDown(2))
		{
			// Get mouse origin
			_mouseOrigin = Input.mousePosition;
			_isZooming = true;
		}

		// Disable movements on button release
		if (!Input.GetMouseButton(0)) _isRotating = false;
		if (!Input.GetMouseButton(1)) _isPanning = false;
		if (!Input.GetMouseButton(2)) _isZooming = false;

		// Rotate camera along X and Y axis
		if (_isRotating)
		{
			Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - _mouseOrigin);

			transform.RotateAround(transform.position, transform.right, -pos.y * _turnSpeed);
			transform.RotateAround(transform.position, Vector3.up, pos.x * _turnSpeed);

		}

		// Move the camera on it's XY plane
		if (_isPanning)
		{
			Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - _mouseOrigin);

			Vector3 move = new Vector3(pos.x * _panSpeed, pos.y * _panSpeed, 0);
			transform.Translate(move, Space.Self);
		}

		// Move the camera linearly along Z axis
		if (_isZooming)
		{
			Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - _mouseOrigin);

			Vector3 move = pos.y * _zoomSpeed * transform.forward;
			transform.Translate(move, Space.World);
		}


		//Key movement
		if (Input.GetKey(KeyCode.RightArrow))
		{
			transform.Translate(new Vector3(_keyPanSpeed * Time.deltaTime, 0, 0));
		}
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			transform.Translate(new Vector3(-_keyPanSpeed * Time.deltaTime, 0, 0));
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
			transform.Translate(new Vector3(0, 0, -_keyZoomSpeed * Time.deltaTime));
		}
		if (Input.GetKey(KeyCode.UpArrow))
		{
			transform.Translate(new Vector3(0, 0, _keyZoomSpeed * Time.deltaTime));
		}
		if (Input.GetKey(KeyCode.Escape))
		{
			BurndownChart.ShowSettings();
		}

		if (!_isPanning && !_isRotating && !_isZooming && BurndownChart.LoadedObjects)
		{
			BurndownChart.SelectedDayKey = null;
			_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(_ray, out _hit))
			{
				if (_hit.collider.name.Contains("Bar"))
				{
					BurndownChart.SelectedDayKey = _hit.collider.GetComponentInParent<Bar>().SelectedBarKey;
				}

			}
		}

	}
}