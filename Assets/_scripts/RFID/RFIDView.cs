using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _scripts.PixelSpriteConsumer;

namespace _scripts.RFID
{
    public class RFIDView : MonoBehaviour
    {
        public GameObject CanvasAccept;
        public GameObject CanvasTutorial;
        public RFIDController RFIDController;

        public TextMeshProUGUI idText;
        public Image creatureImage;
        private PixelSpriteController Psc;

        private void Awake()
        {
            Psc = new PixelSpriteController();
        }

        private void OnEnable()
        {
            EnableTutorial();
        }

        public void NewScan()
        {
            EnableTutorial();
            RFIDController.WaitForNfc();
        }

        private void Start()
        {   
            RFIDController.returnId += (response, error) =>
            {
                EnableAddScreen();
                if ( error != null )
                {
                    Debug.Log("Ocorreram erros ao ler o NFC");
                    Debug.Log($"tipo: {error.type}, menssagem: {error.message}");
                    idText.text = error.message;
                    return;
                }

                idText.text = response;
                var sprite = Psc.GetSprite( response.GetHashCode());
                creatureImage.sprite = sprite;
            };
            RFIDController.WaitForNfc();
        }

        public void EnableTutorial()
        {
            CanvasAccept.SetActive(false);
            CanvasTutorial.SetActive(true);
        }

        public void EnableAddScreen()
        {
            CanvasAccept.SetActive(true);
            CanvasTutorial.SetActive(false);
        }
    }
}