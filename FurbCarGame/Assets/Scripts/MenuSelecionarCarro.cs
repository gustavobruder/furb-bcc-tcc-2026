using Logitech;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuSelecionarCarro : MonoBehaviour
{
    public GameObject[] carros;
    public Button btnEsquerda;
    public Button btnDireita;
    public Button btnFaseTransito1;
    public Button btnFaseTransito2;
    public Button btnFaseEstacionamento1;
    public Button btnFaseEstacionamento2;
    public Button btnFaseEstacionamento3;
    public Button btnFaseCorrida1;
    public Button btnFaseCorrida2;
    public GameObject menuMapeamentoBotoes;

    private int _indiceCarroSelecionado;
    private int _indiceFaseEstacionamentoSelecionada;
    private LogitechGSDK.DIJOYSTATE2ENGINES _logiState;
    private const int INDICE_BTN_DPAD_ESQUERDA = 27000;
    private const int INDICE_BTN_DPAD_DIREITA = 9000;
    private const int INDICE_BTN_MARCHA_BAIXO = 5;
    private const int INDICE_BTN_MARCHA_CIMA = 4;
    private const int INDICE_BTN_OPTIONS = 9;
    private const int INDICE_BTN_X = 0;
    private bool[] _btnsPressionados = new bool[128];
    private uint _dpadAntes;

    private void Start()
    {
        Debug.Log("SteeringInit:" + LogitechGSDK.LogiSteeringInitialize(false));
        _indiceCarroSelecionado = PlayerPrefs.GetInt("indiceCarroSelecionado");
        DefinirBotoesInterativos();
        DefinirCarroSelecionado();
        DefinirBotaoFaseSelecionada();
    }

    private void OnApplicationQuit()
    {
        Debug.Log("SteeringShutdown:" + LogitechGSDK.LogiSteeringShutdown());
    }

    private void Update()
    {
        if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
        {
            _logiState = LogitechGSDK.LogiGetStateUnity(0);
            ProcessarBotoesVolante(_logiState);
        }
        else if (!LogitechGSDK.LogiIsConnected(0))
        {
            Debug.LogWarning("Volante e pedais Logitech não estão conectados.");
        }
    }

    private void ProcessarBotoesVolante(LogitechGSDK.DIJOYSTATE2ENGINES logiState)
    {
        ControlarSelecaoFase(logiState.rgdwPOV[0]);

        if (BtnPressionado(logiState, INDICE_BTN_MARCHA_BAIXO)) Esquerda();
        if (BtnPressionado(logiState, INDICE_BTN_MARCHA_CIMA)) Direita();
        if (BtnPressionado(logiState, INDICE_BTN_OPTIONS)) menuMapeamentoBotoes.SetActive(!menuMapeamentoBotoes.activeSelf);
        if (BtnPressionado(logiState, INDICE_BTN_X)) JogarFaseSelecionada();
    }

    private void ControlarSelecaoFase(uint dpad)
    {
        if (dpad != _dpadAntes)
        {
            switch (dpad)
            {
                case INDICE_BTN_DPAD_ESQUERDA when _indiceFaseEstacionamentoSelecionada > 0:
                    _indiceFaseEstacionamentoSelecionada--;
                    DefinirBotaoFaseSelecionada();
                    break;
                case INDICE_BTN_DPAD_DIREITA when _indiceFaseEstacionamentoSelecionada < 6:
                    _indiceFaseEstacionamentoSelecionada++;
                    DefinirBotaoFaseSelecionada();
                    break;
            }
        }

        _dpadAntes = dpad;
    }

    private void DefinirBotaoFaseSelecionada()
    {
        btnFaseTransito1.interactable = _indiceFaseEstacionamentoSelecionada == 0;
        btnFaseTransito2.interactable = _indiceFaseEstacionamentoSelecionada == 1;
        btnFaseEstacionamento1.interactable = _indiceFaseEstacionamentoSelecionada == 2;
        btnFaseEstacionamento2.interactable = _indiceFaseEstacionamentoSelecionada == 3;
        btnFaseEstacionamento3.interactable = _indiceFaseEstacionamentoSelecionada == 4;
        btnFaseCorrida1.interactable = _indiceFaseEstacionamentoSelecionada == 5;
        btnFaseCorrida2.interactable = _indiceFaseEstacionamentoSelecionada == 6;
    }

    private bool BtnPressionado(LogitechGSDK.DIJOYSTATE2ENGINES logiState, int indiceBtn)
    {
        if (indiceBtn < 0 || indiceBtn >= logiState.rgbButtons.Length)
            return false;

        var btnNaoEstavaPressionadoAntes = !_btnsPressionados[indiceBtn];

        if (logiState.rgbButtons[indiceBtn] == 128)
            _btnsPressionados[indiceBtn] = true;
        else
            _btnsPressionados[indiceBtn] = false;

        var btnEstaPressionadoAgora = _btnsPressionados[indiceBtn];

        return btnNaoEstavaPressionadoAntes && btnEstaPressionadoAgora;
    }

    public void Esquerda()
    {
        if (!btnEsquerda.interactable)
            return;

        _indiceCarroSelecionado--;
        DefinirBotoesInterativos();
        DefinirCarroSelecionado();
        SalvarIndiceCarroSelecionado();
    }

    public void Direita()
    {
        if (!btnDireita.interactable)
            return;

        _indiceCarroSelecionado++;
        DefinirBotoesInterativos();
        DefinirCarroSelecionado();
        SalvarIndiceCarroSelecionado();
    }

    private void DefinirBotoesInterativos()
    {
        btnEsquerda.interactable = _indiceCarroSelecionado > 0;
        btnDireita.interactable = _indiceCarroSelecionado < carros.Length - 1;
    }

    private void DefinirCarroSelecionado()
    {
        for (var i = 0; i < carros.Length; i++)
        {
            carros[i].SetActive(i == _indiceCarroSelecionado);
        }
    }

    private void SalvarIndiceCarroSelecionado()
    {
        PlayerPrefs.SetInt("indiceCarroSelecionado", _indiceCarroSelecionado);
        PlayerPrefs.Save();
    }

    private void JogarFaseSelecionada()
    {
        switch (_indiceFaseEstacionamentoSelecionada)
        {
            case 0: JogarFaseTransito1(); break;
            case 1: JogarFaseTransito2(); break;
            case 2: JogarFaseEstacionamento1(); break;
            case 3: JogarFaseEstacionamento2(); break;
            case 4: JogarFaseEstacionamento3(); break;
            case 5: JogarFaseCorrida1(); break;
            case 6: JogarFaseCorrida2(); break;
        }
    }

    public void JogarFaseTransito1()
    {
        SceneManager.LoadSceneAsync("Fase1");
    }

    public void JogarFaseTransito2()
    {
        SceneManager.LoadSceneAsync("Fase2");
    }

    public void JogarFaseEstacionamento1()
    {
        SceneManager.LoadSceneAsync("Estacionamento1");
    }

    public void JogarFaseEstacionamento2()
    {
        SceneManager.LoadSceneAsync("Estacionamento2");
    }

    public void JogarFaseEstacionamento3()
    {
        SceneManager.LoadSceneAsync("Estacionamento3");
    }

    public void JogarFaseCorrida1()
    {
        SceneManager.LoadSceneAsync("Fase3");
    }

    public void JogarFaseCorrida2()
    {
        SceneManager.LoadSceneAsync("Fase4");
    }
}
