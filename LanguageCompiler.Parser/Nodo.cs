using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageCompiler.Parser
{
    public class Nodo
    {
        private String nombre;
        private List<Nodo> hijos = new List<Nodo>();
        private String valor;
        private int numNodo;

        public Nodo(String nombre)
        {
            this.nombre = nombre;
            hijos = new List<Nodo>();
            this.numNodo = 0;
        }

        public void addHijo(Nodo hijo)
        {
            hijos.Add(hijo);
        }


        public String getNombre()
        {
            return nombre;
        }

        public void setNombre(String n)
        {
            nombre = n;
        }

        public List<Nodo> getHijos()
        {
            return hijos;
        }
        public void setHijos(List<Nodo> hijos)
        {
            this.hijos = hijos;
        }

        public String getValor()
        {
            return valor;
        }

        public void setValor(String valor)
        {
            this.valor = valor;
        }

        public int getNumNodo()
        {
            return numNodo;
        }

        public String toString()
        {

            return nombre;
        }

        public void setNumNodo(int numNodo)
        {
            this.numNodo = numNodo;
        }
    }
}