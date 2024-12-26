using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

enum ShopOption
{
    RESTORE_ARMOR = 0,
    RESTORE_ARROW = 1,
    RESTORE_MANA = 2
}

public class ShopLogic : MonoBehaviour
{
    private KnightController knightController;
    private bool isShowing = false;
    public bool IsShowing
    {
        get { return isShowing; }
        set
        {
            isShowing = value;
            if (isShowing)
            {
                ShowUI();
            }
            else
            {
                HideUI();
            }
        }
    }

    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private Button[] buttonOptions;
    [SerializeField] private CanvasGroup CanvasGroup;
    [SerializeField] private TMP_Text textNotify;

    private Coroutine coroutineNotify;

    private void Start()
    {
        for (int i = 0; i < buttonOptions.Length; i++)
        {
            int index = i;
            buttonOptions[index].onClick.AddListener(() => HandleButtonOption_Clicked(index));
        }

        IsShowing = false;
        textNotify.gameObject.SetActive(false);
        coroutineNotify = null;

        GameObject player = GameObject.FindWithTag("Player");
        knightController = player.GetComponent<KnightController>();
    }

    private void OnDestroy()
    {
        for (int i = 0; i < buttonOptions.Length; i++)
        {
            buttonOptions[i].onClick.RemoveAllListeners();
        }
    }

    private void Update()
    {
        if (boxCollider.IsTouching(knightController.GetComponent<Collider2D>()) && Input.GetKeyDown(KeyCode.S))
        {
            IsShowing = !IsShowing;
            knightController.enabled = !knightController.enabled;
            knightController.rigidbody2d.linearVelocity = Vector3.zero;
        }
    }

    private void HandleButtonClose_CLicked()
    {
        IsShowing = false;
        knightController.enabled = true;
    }

    private void HandleButtonOption_Clicked(int option)
    {
        if (knightController.currentCoin <= 0)
        {
            if (coroutineNotify != null)
            {
                StopCoroutine(coroutineNotify);
                textNotify.gameObject.SetActive(false);
            }
            coroutineNotify = StartCoroutine(Notify("You don't have enough coins!"));
            return;
        }

        switch (option)
        {
            case (int)ShopOption.RESTORE_ARMOR:
                {
                    if (knightController.armor < knightController.maxArmor)
                    {
                        knightController.currentCoin--;
                        knightController.armor = knightController.maxArmor;
                        break;
                    }
                    else
                    {
                        coroutineNotify = StartCoroutine(Notify("You already have full armor!"));
                        break;
                    }
                }
            case (int)ShopOption.RESTORE_ARROW:
                {
                    if (knightController.arrows < knightController.maxArrows)
                    {
                        knightController.currentCoin--;
                        knightController.arrows = Math.Min(knightController.arrows + 1, knightController.maxArrows);
                        break;
                    }
                    else
                    {
                        coroutineNotify = StartCoroutine(Notify("You cannot carry more arrows!"));
                        break;
                    }
                }
            case (int)ShopOption.RESTORE_MANA:
                if (knightController.wizardUpgrade < 5)
                {
                    knightController.currentCoin--;
                    knightController.wizardUpgrade = knightController.wizardUpgrade + 1;
                    break;
                }
                else
                {
                    coroutineNotify = StartCoroutine(Notify("Your spell can't be upgraded more!"));
                    break;
                }
            default:
                break;
        }
    }

    private void ShowUI()
    {
        CanvasGroup.alpha = 1;
        CanvasGroup.blocksRaycasts = true;
        CanvasGroup.interactable = true;
    }

    private void HideUI()
    {
        CanvasGroup.alpha = 0;
        CanvasGroup.blocksRaycasts = false;
        CanvasGroup.interactable = false;
        textNotify.gameObject.SetActive(false);
    }

    private IEnumerator Notify(string notice)
    {
        textNotify.text = notice;
        textNotify.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        textNotify.gameObject.SetActive(false);
    }
}
