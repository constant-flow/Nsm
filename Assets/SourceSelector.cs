using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System    ;
using Klak.Ndi;

namespace Nsm {

public sealed class SourceSelector : MonoBehaviour
{
    [SerializeField] Dropdown _dropdown = null;

    NdiReceiver _receiver;
    List<string> _sourceNames;
    bool _disableCallback;

    // HACK: Assuming that the dropdown has more than
    // three child objects only while it's opened.
    bool IsOpened => _dropdown.transform.childCount > 3;

    void Start() {
        _receiver = GetComponent<NdiReceiver>();

        // check for available arguments
        readArguments();
    }

    void Update()
    {
        // readArguments();
        // Do nothing if the menu is opened.
        if (IsOpened) return;

        // NDI source name retrieval
        _sourceNames = NdiFinder.sourceNames.ToList();

        // Currect selection
        var index = _sourceNames.IndexOf(_receiver.ndiName);

        // Append the current name to the list if it's not found.
        if (index < 0)
        {
            index = _sourceNames.Count;
            _sourceNames.Add(_receiver.ndiName);

            readArguments();
        }

        // Disable the callback while updating the menu options.
        _disableCallback = true;

        // Menu option update
        _dropdown.ClearOptions();
        _dropdown.AddOptions(_sourceNames);
        _dropdown.value = index;
        _dropdown.RefreshShownValue();

        // Resume the callback.
        _disableCallback = false;
    }

    public void readArguments() {
        string[] args = System.Environment.GetCommandLineArgs ();
        string input = "";
        bool takeNextArgumentAsValue = false;
        for (int i = 0; i < args.Length; i++) {
            // Debug.LogError ("ARG " + i + ": " + args );
            for (int subi = 0; subi < args.Length; subi++) {

                // Debug.Log ("'" + args[subi] +"'");

                if(String.Compare(args[subi],"-ndiChannel")==0) {
                    takeNextArgumentAsValue = true;
                    continue;
                }

                if(takeNextArgumentAsValue) {
                    //  Debug.Log ("Argument value: " + args[subi] );
                     takeNextArgumentAsValue = false;


                    string sourceToSelect = args[subi];
                    // String sourceToSelect = new String("DE_C02D66WXMD6T (Scan Converter)");
                    // String sourceToSelect = new String("DE_C02D66WXMD6T (Test Patterns)");

                     updateDropDown(sourceToSelect);
                }
            }
        }
    }

    public void updateDropDown(String valueToSet) {
        // Currect selection
        var index = _sourceNames.IndexOf(valueToSet);

        // Append the current name to the list if it's not found.
        if (index < 0)
        {
            Debug.Log("❌ NDI channel not found: " + valueToSet);
            return;
        } else {

            Debug.Log("✅ NDI channel set successfully: " + valueToSet + " index:" + index);
        }

        // Disable the callback while updating the menu options.
        _disableCallback = true;

        _dropdown.value = index;
        _receiver.ndiName = _sourceNames[index];

        var go = _dropdown.gameObject;
        go.SetActive(false);

        _dropdown.RefreshShownValue();

        // Resume the callback.
        _disableCallback = false;
    }

    public void OnChangeValue(int value)
    {
        if (_disableCallback) return;
        _receiver.ndiName = _sourceNames[value];
    }

    public void OnClickEmptyArea()
    {
        var go = _dropdown.gameObject;
        go.SetActive(!go.activeSelf);
        Cursor.visible = go.activeSelf;
    }
}

} // namespace Nsm
