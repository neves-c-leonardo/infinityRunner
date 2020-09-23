using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControleJogador : MonoBehaviour
{
    public Rigidbody jogador;
    public GameObject cenario;
    public float velocidadeCenario;
    public float distanciaRaia;
    private int raiaAtual;

    private Vector3 target;
    private Vector2 initialPosition;

    public GameObject chao;
    public GameObject bloco;
    public GameObject moeda;

    private int estagioAtual = 1;

    private AudioSource somMoeda;
    private AudioSource somColisao;
    private AudioSource somFundo;
    private AudioSource somFundo2;
    public Text txtPontos;

    private bool jump = false;
    private int pontos = 0;

    private bool isGameOver = false;
    public Text txtGameOver;

    void Start(){
        raiaAtual = 1;
        target = jogador.transform.position;
        montarCenario();

        somMoeda = GetComponents<AudioSource>()[0];
        somColisao = GetComponents<AudioSource>()[1];
        somFundo = GetComponents<AudioSource>()[2];
        somFundo2 = GetComponents<AudioSource>()[3];

        txtPontos.text = ""+pontos;
        somFundo.Play();
    }

    void Update(){

        if(isGameOver){
            return;
        }

        transform.Rotate(new Vector3(90,0,0)*Time.deltaTime);

        int novaRaia = -1;
        bool jump = false;

        // teclado
        if(Input.GetKeyDown(KeyCode.RightArrow) && raiaAtual<2){
            novaRaia = raiaAtual + 1;
        } else if(Input.GetKeyDown(KeyCode.LeftArrow) && raiaAtual>0){
            novaRaia = raiaAtual -1;
        }if (Input.GetKeyDown(KeyCode.Space)){
            jump = true;
        }

        // mouse
        if(Input.GetMouseButtonDown(0)){
            initialPosition = Input.mousePosition;
        }  else if(Input.GetMouseButtonUp(0)){
            if(Input.mousePosition.x > initialPosition.x && raiaAtual < 2){
                novaRaia = raiaAtual + 1;
            } else if(Input.mousePosition.x < initialPosition.x && raiaAtual > 0){
                novaRaia = raiaAtual - 1;
            }
            if (Input.mousePosition.y > initialPosition.y){
                jump = true;
            }
        }

        // touch
        if(Input.touchCount>=1){
            if(Input.GetTouch(0).phase == TouchPhase.Began){
                initialPosition = Input.GetTouch(0).position;
            } else if(Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled){
                if(Input.GetTouch(0).position.x > initialPosition.x && raiaAtual < 2){
                    novaRaia = raiaAtual + 1;
                } else if(Input.GetTouch(0).position.x > initialPosition.x && raiaAtual > 0){
                    novaRaia = raiaAtual - 1;
                }
                if (Input.GetTouch(0).position.y > initialPosition.y){
                    jump = true;
                }
            }
        }

        if(novaRaia>=0){
            raiaAtual = novaRaia;
            target = new Vector3((raiaAtual-1)*distanciaRaia,jogador.transform.position.y,jogador.transform.position.z);
        }
        // suavizando o movimento
        if(jogador.transform.position.x != target.x){
            jogador.transform.position = Vector3.Lerp(jogador.transform.position, target, 5*Time.deltaTime);
        }

        if(jump){
            if (jogador.transform.position.y < 4.5F) {
                target.y = 3.0F;
                jogador.transform.position = Vector3.Lerp(jogador.transform.position, target, 5 * Time.deltaTime);
            } else {
                jump = false;
            };
        } else if (jump == false && jogador.transform.position.y > 4.50) {
            target.y = 1.0F;
            jogador.transform.position = Vector3.Lerp(jogador.transform.position, target, 5 * Time.deltaTime);
        } else if (jogador.transform.position.x != target.x) {
            jogador.transform.position = Vector3.Lerp(jogador.transform.position, target, 5 * Time.deltaTime);
        }

        // move o chao
        velocidadeCenario += (Time.deltaTime * 0.1F);
        cenario.transform.Translate(0,0,velocidadeCenario*Time.deltaTime * -1);

        float cenarioz = cenario.transform.position.z;
        float estagio = Mathf.Floor((cenario.transform.position.z-50.0F)/-100.0F) +1;
        if(estagio>estagioAtual){
            GameObject chao2 = Instantiate(chao);
            float chao2z = ((100*estagioAtual)+50) + cenario.transform.position.z;
            chao2.transform.SetParent(cenario.transform);
            chao2.transform.position = new Vector3(chao.transform.position.x,chao.transform.position.y,chao2z);
            estagioAtual++;
            montarCenario();
        }
    }

    void montarCenario(){
        for(int i=2;i<10;i++){
            int elemento1 = Random.Range(0,3);
            int elemento2 = Random.Range(0,3);
            int elemento3 = Random.Range(0,3);
            //0 = nada, 1 = bloco, 2 = moeda

            if(elemento1==1 && elemento2==1 && elemento3==1){
                elemento2 = 0;
            }
            if(elemento1==1){
                instanciarBloco(i,0);
            } else if(elemento1==2){ instanciarMoeda(i,0);}
            if(elemento2==1){
                instanciarBloco(i,1);
            } else if(elemento2==2){ instanciarMoeda(i,1);}
            if(elemento3==1){
                instanciarBloco(i,2);
            } else if(elemento3==2){ instanciarMoeda(i,2);}
        }
    }


    void instanciarBloco(int posicaoz, int posicaox) {
        GameObject bloco2 = Instantiate(bloco);
        float posz = ((10*posicaoz)+((estagioAtual-1)*100)) +
        cenario.transform.position.z;
        float posx = (posicaox-1)*distanciaRaia;
        bloco2.transform.SetParent(cenario.transform);
        bloco2.transform.position = new Vector3(posx,1.5F, posz);
    }
    void instanciarMoeda(int posicaoz, int posicaox) {
        GameObject moeda2 = Instantiate(moeda);
        float posz = ((10*posicaoz)+((estagioAtual-1)*100)) +
        cenario.transform.position.z;
        float posx = (posicaox-1)*distanciaRaia;
        moeda2.transform.SetParent(cenario.transform);
        moeda2.transform.position = new Vector3(posx,1.5F, posz);
    }
    
   private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Moeda"))
        {
            somMoeda.Play();
            Destroy(other.gameObject);
            pontos++;
            txtPontos.text = "" + pontos;
        }
        if (other.gameObject.CompareTag("Diamante"))
        {
            somFundo.Pause();
            somFundo2.Play();
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Mola"))
        {
            
            somFundo.Pause();
        }
        if (other.gameObject.CompareTag("Obstaculo"))
        {
            somFundo.Pause();
            somFundo2.Pause();
            txtGameOver.text = "GAME OVER";
            isGameOver = true;
            somColisao.Play();
            Invoke("GameOver", 5);
        }
    }

    void GameOver()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}