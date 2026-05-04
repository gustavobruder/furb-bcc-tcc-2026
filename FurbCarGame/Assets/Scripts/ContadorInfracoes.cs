using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContadorInfracoes : MonoBehaviour
{
    public Text textoInfracoes;
    public Text textoPontos;
    private List<Infracao> _infracoes = new List<Infracao>();
    private int _quantidadeInfracoes = 0;
    private int _quantidadePontos = 0;

    public void AdicionarInfracaoMedia(string descricao)
    {
        AdicionarInfracao(descricao, nivel: "Média", pontos: 4);
        DefinirTextoInfracoes();
    }

    public void AdicionarInfracaoGrave(string descricao)
    {
        AdicionarInfracao(descricao, nivel: "Grave", pontos: 5);
        DefinirTextoInfracoes();
    }

    public void AdicionarInfracaoGravissima(string descricao)
    {
        AdicionarInfracao(descricao, nivel: "Gravíssima", pontos: 7);
        DefinirTextoInfracoes();
    }

    private void AdicionarInfracao(string descricao, string nivel, short pontos)
    {
        _infracoes.Add(new Infracao(descricao, nivel, pontos));
        _quantidadeInfracoes++;
        _quantidadePontos += pontos;
    }

    private void DefinirTextoInfracoes()
    {
        textoInfracoes.text = $"Infrações: {_quantidadeInfracoes}";
        textoPontos.text = $"Pontos: {_quantidadePontos}";
    }

    private class Infracao
    {
        public string Descricao { get; set; }
        public string Nivel { get; set; }
        public short Pontos { get; set; }

        public Infracao(string descricao, string nivel, short pontos)
        {
            Descricao = descricao;
            Nivel = nivel;
            Pontos = pontos;
        }
    }
}
