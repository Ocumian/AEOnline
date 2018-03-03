using AEOnline.Models;
using AEOnline.Models.WebModels;
using GMap.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AEOnline.ClasesAdicionales
{
    public class HistorialesManager
    {


        public static List<PuntoGrafico> CrearHistorialVelocidadEjemplo()
        {
            List<PuntoGrafico> historial = new List<PuntoGrafico>();

            PuntoGrafico h1 = new PuntoGrafico()
            {
                FechaHora = new DateTime(2018, 1, 2, 15, 30, 50),
                Valor = 5
            };
            historial.Add(h1);

            PuntoGrafico h2 = new PuntoGrafico()
            {
                FechaHora = new DateTime(2018, 1, 2, 16, 50, 50),
                Valor = 8
            };
            historial.Add(h2);

            PuntoGrafico h3 = new PuntoGrafico()
            {
                FechaHora = new DateTime(2018, 1, 2, 20, 25, 42),
                Valor = 2
            };
            historial.Add(h3);

            PuntoGrafico h4 = new PuntoGrafico()
            {
                FechaHora = new DateTime(2018, 1, 2, 20, 42, 56),
                Valor = 3.4f
            };
            historial.Add(h4);

            PuntoGrafico h5 = new PuntoGrafico()
            {
                FechaHora = new DateTime(2018, 1, 2, 21, 10, 56),
                Valor = 8.2f
            };
            historial.Add(h5);

            PuntoGrafico h6 = new PuntoGrafico()
            {
                FechaHora = new DateTime(2018, 1, 2, 21, 16, 26),
                Valor = 5
            };
            historial.Add(h6);

            PuntoGrafico h7 = new PuntoGrafico()
            {
                FechaHora = new DateTime(2018, 1, 2, 21, 32, 15),
                Valor = 4.2f
            };
            historial.Add(h7);

            PuntoGrafico h8 = new PuntoGrafico()
            {
                FechaHora = new DateTime(2018, 1, 2, 21, 43, 11),
                Valor = 5.12f
            };
            historial.Add(h8);


            return historial;

        }

        public static List<PuntoGrafico> CrearHistorialEnergiaEjemplo()
        {
            List<PuntoGrafico> historial = new List<PuntoGrafico>();

            PuntoGrafico h1 = new PuntoGrafico()
            {
                FechaHora = new DateTime(2018, 1, 2, 15, 30, 50),
                Valor = 23
            };
            historial.Add(h1);

            PuntoGrafico h2 = new PuntoGrafico()
            {
                FechaHora = new DateTime(2018, 1, 2, 16, 50, 50),
                Valor = 43
            };
            historial.Add(h2);

            PuntoGrafico h3 = new PuntoGrafico()
            {
                FechaHora = new DateTime(2018, 1, 2, 20, 25, 42),
                Valor = 32
            };
            historial.Add(h3);

            PuntoGrafico h4 = new PuntoGrafico()
            {
                FechaHora = new DateTime(2018, 1, 2, 20, 42, 56),
                Valor = 55
            };
            historial.Add(h4);

            PuntoGrafico h5 = new PuntoGrafico()
            {
                FechaHora = new DateTime(2018, 1, 2, 21, 10, 56),
                Valor = 61
            };
            historial.Add(h5);

            PuntoGrafico h6 = new PuntoGrafico()
            {
                FechaHora = new DateTime(2018, 1, 2, 21, 16, 26),
                Valor = 32
            };
            historial.Add(h6);

            PuntoGrafico h7 = new PuntoGrafico()
            {
                FechaHora = new DateTime(2018, 1, 2, 21, 32, 15),
                Valor = 44
            };
            historial.Add(h7);

            PuntoGrafico h8 = new PuntoGrafico()
            {
                FechaHora = new DateTime(2018, 1, 2, 21, 43, 11),
                Valor = 51
            };
            historial.Add(h8);

            return historial;
        }

        public static List<HistorialPosicion> CrearHistorialPosicionejemplo()
        {
            List<HistorialPosicion> ejemplo = new List<HistorialPosicion>();

            HistorialPosicion h1 = new HistorialPosicion()
            {
                FechaHora = new DateTime(2018, 1, 1, 8, 0, 0),
                Latitud = -40.573431,
                Longitud = -73.112800
            };
            ejemplo.Add(h1);

            HistorialPosicion h2 = new HistorialPosicion()
            {
                FechaHora = new DateTime(2018, 1, 1, 9, 0, 0),
                Latitud = -40.572127,
                Longitud = -73.129794
            };
            ejemplo.Add(h2);

            HistorialPosicion h3 = new HistorialPosicion()
            {
                FechaHora = new DateTime(2018, 1, 1, 10, 0, 0),
                Latitud = -40.577342,
                Longitud = -73.129236
            };
            ejemplo.Add(h3);

            HistorialPosicion h4 = new HistorialPosicion()
            {
                FechaHora = new DateTime(2018, 1, 1, 11, 0, 0),
                Latitud = -40.577244,

                Longitud = -73.138291
            };
            ejemplo.Add(h4);

            HistorialPosicion h5 = new HistorialPosicion()
            {
                FechaHora = new DateTime(2018, 1, 1, 12, 0, 0),
                Latitud = -40.568476,

                Longitud = -73.139021
            };
            ejemplo.Add(h5);

            HistorialPosicion h6 = new HistorialPosicion()
            {
                FechaHora = new DateTime(2018, 1, 1, 13, 0, 0),
                Latitud = -40.567335,

                Longitud = -73.144943
            };
            ejemplo.Add(h6);

            HistorialPosicion h7 = new HistorialPosicion()
            {
                FechaHora = new DateTime(2018, 1, 1, 14, 30, 0),
                Latitud = -40.570464,
                Longitud = -73.157346
            };
            ejemplo.Add(h7);

            return ejemplo;
        }


        public static List<PuntoGrafico> FiltrarPuntosGraficoPorFecha(List<PuntoGrafico> _lista, DateTime _fecha)
        {
            List<PuntoGrafico> filtrado = new List<PuntoGrafico>();

            foreach (PuntoGrafico pg in _lista)
            {
                if (pg.FechaHora.Date == _fecha.Date)
                {
                    filtrado.Add(pg);
                }
            }

            return filtrado;
        }

        public static List<PuntoGrafico> FiltrarPuntosGraficoPorHoras(List<PuntoGrafico> _lista, DateTime _desde, DateTime _hasta)
        {
            List<PuntoGrafico> filtrado = new List<PuntoGrafico>();

            foreach (PuntoGrafico pg in _lista)
            {
                if(pg.FechaHora >= _desde && pg.FechaHora <= _hasta)
                {
                    filtrado.Add(pg);
                }
            }

            return filtrado;
        }

        public static List<HistorialPosicion> FiltrarHistorialPosicionPorFecha(List<HistorialPosicion> _lista, DateTime _fecha)
        {
            List<HistorialPosicion> filtrado = new List<HistorialPosicion>();

            foreach (HistorialPosicion hp in _lista)
            {
                if (hp.FechaHora.Date == _fecha)
                {
                    filtrado.Add(hp);
                }
            }

            return filtrado;
        }

        public static List<HistorialPosicion> FiltrarHistorialPosicionPorHoras(List<HistorialPosicion> _lista, DateTime _desde, DateTime _hasta)
        {
            List<HistorialPosicion> filtrado = new List<HistorialPosicion>();

            foreach (HistorialPosicion hp in _lista)
            {
                if (hp.FechaHora >= _desde && hp.FechaHora <= _hasta)
                {
                    filtrado.Add(hp);
                }
            }

            return filtrado;
        }


        public static float BuscarValorMaximo(List<PuntoGrafico> _lista)
        {
            float maxima = 0;

            for (int i = 0; i < _lista.Count; i++)
            {
                if (_lista[i].Valor > maxima)
                {
                    maxima = _lista[i].Valor;
                }
            }
            return maxima;
        }

        public static HistorialWeb PrepararHistorialVelocidad(ProyectoAutoContext _db, HistorialWeb _HW, DateTime _fecha, int _idAuto)
        {
            //historial web ya llega creado (con ciertos datos genericos como fecha y patente)

            //Ordenar historiales velocidad
            //transformarlos apuntos grafico
            //Remover repetidos si se puede
            //asignarlos al historial web

            Auto auto = _db.Autos.Where(a => a.Id == _idAuto).FirstOrDefault();

            if (auto == null)
                return new HistorialWeb();

            HistorialDiario historialHoy = auto.HistorialesDiarios
                    .Where(h => h.Fecha.Year == _fecha.Year
                    && h.Fecha.Month == _fecha.Month
                    && h.Fecha.Day == _fecha.Day).FirstOrDefault();

            if (historialHoy == null)
            {
                _HW.historialesVelocidad = new List<PuntoGrafico>();
                return (_HW);
            }


            List<HistorialVelocidad> histVelocidad = historialHoy.historialesVelocidad.OrderBy(h => h.HoraRegistro).ToList();


            #region HistorialesComparables

            List<HistorialWeb.TiposHistorial> historialesComparables = new List<HistorialWeb.TiposHistorial>();
            historialesComparables.Add(HistorialWeb.TiposHistorial.Energia);
            _HW.historialesComparables = historialesComparables;

            List<HistorialEnergia> histEnergia = historialHoy.historialesEnergia.OrderBy(h => h.HoraRegistro).ToList();

            #endregion


            List<PuntoGrafico> puntosVelocidad = VelocidadAPuntosGrafico(histVelocidad);
            List<PuntoGrafico> puntosEnergia = EnergiaAPuntosGrafico(histEnergia);

            if (histVelocidad.Count == 0)
            {
                puntosVelocidad = CrearHistorialVelocidadEjemplo();
                puntosEnergia = CrearHistorialEnergiaEjemplo();
            }

            _HW.historialesVelocidad = puntosVelocidad;
            _HW.historialesEnergia = puntosEnergia;
            _HW.ValorMaximo = BuscarValorMaximo(puntosVelocidad);


            return _HW;
        }

        public static HistorialWeb PrepararHistorialEnergia(ProyectoAutoContext _db, HistorialWeb _HW, DateTime _fecha, int _idAuto)
        {
            Auto auto = _db.Autos.Where(a => a.Id== _idAuto).FirstOrDefault();

            if (auto == null)
                return new HistorialWeb();

            HistorialDiario historialHoy = auto.HistorialesDiarios
                    .Where(h => h.Fecha.Year == _fecha.Year
                    && h.Fecha.Month == _fecha.Month
                    && h.Fecha.Day == _fecha.Day).FirstOrDefault();

            if (historialHoy == null)
            {
                _HW.historialesEnergia = new List<PuntoGrafico>();
                return (_HW);
            }

            List<HistorialEnergia> histEnergia = historialHoy.historialesEnergia.OrderBy(h => h.HoraRegistro).ToList();

            #region HistorialesComparables
            List<HistorialWeb.TiposHistorial> historialesComparables = new List<HistorialWeb.TiposHistorial>();
            historialesComparables.Add(HistorialWeb.TiposHistorial.Velocidad);
            _HW.historialesComparables = historialesComparables;

            List<HistorialVelocidad> histVelocidad = historialHoy.historialesVelocidad.OrderBy(h => h.HoraRegistro).ToList();
            #endregion

            List<PuntoGrafico> puntosEnergia = EnergiaAPuntosGrafico(histEnergia);
            List<PuntoGrafico> puntosVelocidad = VelocidadAPuntosGrafico(histVelocidad);

            if (histVelocidad.Count == 0)
            {
                puntosVelocidad = CrearHistorialVelocidadEjemplo();
                puntosEnergia = CrearHistorialEnergiaEjemplo();
            }

            _HW.historialesEnergia = puntosEnergia;
            _HW.historialesVelocidad = puntosVelocidad;

            return _HW;
        }

        public static HistorialWeb PrepararHistorialPosicion(ProyectoAutoContext _db, HistorialWeb _HW, DateTime _fecha, int _idAuto)
        {
            Auto auto = _db.Autos.Where(a => a.Id == _idAuto).FirstOrDefault();

            if (auto == null)
                return new HistorialWeb();

            HistorialDiario historialHoy = auto.HistorialesDiarios
                .Where(h => h.Fecha.Year == _fecha.Year
                && h.Fecha.Month == _fecha.Month
                && h.Fecha.Day == _fecha.Day).FirstOrDefault();

            DateTime fechaEjemplo = new DateTime(2018, 1, 1);
            if(_fecha.Date == fechaEjemplo.Date)
            {
                _HW.historialesPosicion = CrearHistorialPosicionejemplo();
                return _HW;
            }

            if (historialHoy == null)
            {
                _HW.historialesPosicion = new List<HistorialPosicion>();
                return _HW;
            }

            List<HistorialPosicion> historiales = historialHoy.historialesPosicion.OrderBy(h => h.FechaHora).ToList();
            _HW.historialesPosicion = historiales;

            return _HW;

        }

        public static List<HistorialPosicion> ProcesarHistorialPosicion(ProyectoAutoContext _db, HistorialDiario _histDiario, List<HistorialPosicion> _listaAprocesar)
        {
            //se recorren los historiales
            //se verifica si hay algun punto no procesado
            //si un punto no está procesado, significa que el siguiente tampoco lo está y hay que hacer una ruta entre ambos
            //al procesarse, se deja el ultimo punto de la ruta no procesado

            List<HistorialPosicion> puntosValidos = new List<HistorialPosicion>();

            HistorialPosicion inicio;
            HistorialPosicion final;
            HistorialPosicion postFinal = null;

            int indexExtraInicio = 0;

            //Se guardan los puntos validos (2 puntos seguidos en misma calle, etc)
            //Esos seran los puntos en que se crearan rutas entre uno y otros
            for (int i = 0; i < _listaAprocesar.Count; i++)
            {
                //Si el punto actual no es el final se procesa

                if(_listaAprocesar[i].Procesado)
                {
                    puntosValidos.Add(_listaAprocesar[i]);
                }
                else if((i + indexExtraInicio + 1) < _listaAprocesar.Count)
                {
                    int indexInicio = i + indexExtraInicio;

                    inicio = _listaAprocesar[indexInicio];
                    final = _listaAprocesar[indexInicio + 1];

                    bool inicioFinalValido = false;
                    //si punto final y postfinal son la misma calle es un punto valido
                    int cFinal = 1;
                    while (inicioFinalValido == false)
                    {
                        postFinal = null;
                        if (indexInicio + cFinal + 1 < _listaAprocesar.Count)
                        {
                            final = _listaAprocesar[indexInicio + cFinal];
                            postFinal = _listaAprocesar[indexInicio + cFinal + 1];

                            if (postFinal.Procesado == true)
                            {
                                postFinal = null;
                                indexExtraInicio += cFinal - 1;
                                inicioFinalValido = true;
                            }
                            else
                            {
                                string calle1 = Posicion.ObtenerCalle(final.Latitud, final.Longitud);
                                string calle2 = Posicion.ObtenerCalle(postFinal.Latitud, postFinal.Longitud);

                                if (calle1 == calle2 || calle1 == "" || calle2 == "")
                                {
                                    //Si es valido, queda "final" como punto de llegada y salel del while
                                    indexExtraInicio += cFinal;
                                    inicioFinalValido = true;
                                }
                                cFinal += 1;
                            } 
                        }
                        else
                        {
                            //No hay un punto postfinal porque se llega al final de la lista y se designa este
                            final = _listaAprocesar[indexInicio + cFinal];
                            inicioFinalValido = true;
                        }
                    }

                    //Hay riesgo que si una comparacion de calle da vacio, se tendran que incluir esos puntos
                    if(i == 0)
                        puntosValidos.Add(inicio);
                    puntosValidos.Add(final);
                    if(postFinal != null)
                        puntosValidos.Add(postFinal);

                }
            }


            //Se crean las rutas entre uno y otros (siempre deberia haber para crearlos de par en par)
            //se debe asignar las horas a los puntos intermedios creados
            //luego de terminar se deben eliminar _listaAprocesar de la base de datos
            //se agrega la nueva lista a la base de datos

            //puntosValidos = _listaAprocesar; // TESTEANDO

            List<HistorialPosicion> puntosProcesados = new List<HistorialPosicion>();
            HistorialPosicion inicioRuta;
            HistorialPosicion finalRuta;
            for (int i = 0; i < puntosValidos.Count; i++)
            {
                inicioRuta = puntosValidos[i];

                if (inicioRuta.Procesado == true || i+1 >= puntosValidos.Count)
                {
                    puntosProcesados.Add(inicioRuta);
                    continue;
                }

                //deberia siempre el final ser tambien no procesado porque se agregan en pares
                finalRuta = puntosValidos[i + 1];

                GDirections direccion = Posicion.ObtenerDireccion(inicioRuta.Latitud, inicioRuta.Longitud, finalRuta.Latitud, finalRuta.Longitud);

                if(direccion != null)
                {
                    //Se crea la ruta
                    //Se asigna una hora correspondiente a cada punto de la ruta
                    //Se agrega a la lista con todos los puntos con el campo procesado en true

                    List<PointLatLng> ruta = direccion.Route;
                    DateTime horaInicio = inicioRuta.FechaHora;
                    DateTime horaFinal = finalRuta.FechaHora;

                    double diferencia = (horaFinal - horaInicio).TotalSeconds;
                    int secIntervalo = Convert.ToInt32(diferencia / ruta.Count);

                    TimeSpan tiempoSumar = new TimeSpan(0, 0, secIntervalo);
                    TimeSpan c = new TimeSpan(0,0,0);

                    for (int z = 0; z < ruta.Count; z++)
                    {
                        HistorialPosicion histProc = new HistorialPosicion()
                        {
                            Latitud = ruta[z].Lat,
                            Longitud = ruta[z].Lng,
                            FechaHora = horaInicio + c,
                            Procesado = true
                        };
                        puntosProcesados.Add(histProc);
                        c += tiempoSumar;
                    }

                }
                else
                {
                    //se agregan a la lista con los campos "Procesado" aún en false
                    puntosProcesados.Add(inicioRuta);
                    puntosProcesados.Add(finalRuta);
                }
            }


            //Se eliminan los puntos anteriores y se reemplazan por lo procesados

            _db.HistorialesPosicion.RemoveRange(_listaAprocesar);
            _db.SaveChanges();
            _histDiario.historialesPosicion.AddRange(puntosProcesados);
            _db.SaveChanges();

            return puntosProcesados;
            //return puntosValidos;
        }


        public static List<HistorialWeb> ObtenerHistorialesFlota(ProyectoAutoContext _db, DateTime _fecha, int _idFlota)
        {
            Flota flota = _db.Flotas.Where(f => f.Id == _idFlota).FirstOrDefault();

            List<HistorialWeb> historialesFlota = new List<HistorialWeb>();

            //List<HistorialVelocidad> histVel;
            //List<HistorialEnergia> histEner;
            //List<HistorialPosicion> histPos;

            //foreach(Auto a in flota.Autos)
            //{
            //    histVel = a.HistorialVelocidad.Where(h => h.FechaHora.Year == _fecha.Year
            //                && h.FechaHora.Month == _fecha.Month
            //                && h.FechaHora.Day == _fecha.Day).OrderBy(h => h.FechaHora).ToList();

            //    histEner = a.HistorialesEnergia.Where(h => h.FechaHora.Year == _fecha.Year
            //                && h.FechaHora.Month == _fecha.Month
            //                && h.FechaHora.Day == _fecha.Day).OrderBy(h => h.FechaHora).ToList();

            //    histPos = a.HistorialesPosicion.Where(h => h.FechaHora.Year == _fecha.Year
            //                && h.FechaHora.Month == _fecha.Month
            //                && h.FechaHora.Day == _fecha.Day).OrderBy(h => h.FechaHora).ToList();

            //    HistorialWeb hw = new HistorialWeb();
            //    hw.PatenteAuto = a.Patente;
            //    hw.FechaMostrar = _fecha;

            //    hw.historialesVelocidad = histVel;
            //    hw.historialesEnergia = histEner;
            //    hw.historialesPosicion = histPos;

            //    historialesFlota.Add(hw);
            //}


            return historialesFlota;
        }


        #region Tranformaciones a Puntosgrafico

        public static List<PuntoGrafico> VelocidadAPuntosGrafico(List<HistorialVelocidad> _lista)
        {
            List<PuntoGrafico> puntos = new List<PuntoGrafico>();

            for (int i = 0; i < _lista.Count; i++)
            {
                PuntoGrafico puntoInicio = new PuntoGrafico()
                {
                    Valor = _lista[i].ValorInicio,
                    FechaHora = _lista[i].HoraInicio
                };

                PuntoGrafico puntoFinal = new PuntoGrafico()
                {
                    Valor = _lista[i].ValorFinal,
                    FechaHora = _lista[i].HoraFinal
                };

                PuntoGrafico puntoMenor = new PuntoGrafico()
                {
                    Valor = _lista[i].ValorMenor,
                    FechaHora = _lista[i].HoraMenor
                };

                PuntoGrafico puntoMayor = new PuntoGrafico()
                {
                    Valor = _lista[i].ValorMayor,
                    FechaHora = _lista[i].HoraMayor
                };

                PuntoGrafico puntoMitad = new PuntoGrafico()
                {
                    Valor = _lista[i].ValorMitad,
                    FechaHora = _lista[i].HoraMitad
                };

                PuntoGrafico punto1Cuarto = new PuntoGrafico()
                {
                    Valor = _lista[i].ValorUnCuarto,
                    FechaHora = _lista[i].HoraUnCuarto
                };

                PuntoGrafico punto3cuartos = new PuntoGrafico()
                {
                    Valor = _lista[i].ValorTresCuartos,
                    FechaHora = _lista[i].HoraTresCuartos
                };

                puntos.Add(puntoInicio);
                puntos.Add(puntoFinal);
                puntos.Add(puntoMenor);
                puntos.Add(puntoMayor);
                puntos.Add(punto1Cuarto);
                puntos.Add(puntoMitad);
                puntos.Add(punto3cuartos);

            }

            puntos = puntos.Distinct().ToList();
            puntos = puntos.OrderBy(p => p.FechaHora).ToList();

            return puntos;
        }

        public static List<PuntoGrafico> EnergiaAPuntosGrafico(List<HistorialEnergia> _lista)
        {
            List<PuntoGrafico> puntos = new List<PuntoGrafico>();

            for (int i = 0; i < _lista.Count; i++)
            {
                PuntoGrafico puntoInicio = new PuntoGrafico()
                {
                    Valor = _lista[i].ValorInicio,
                    FechaHora = _lista[i].HoraInicio
                };

                PuntoGrafico puntoFinal = new PuntoGrafico()
                {
                    Valor = _lista[i].ValorFinal,
                    FechaHora = _lista[i].HoraFinal
                };

                PuntoGrafico puntoMenor = new PuntoGrafico()
                {
                    Valor = _lista[i].ValorMenor,
                    FechaHora = _lista[i].HoraMenor
                };

                PuntoGrafico puntoMayor = new PuntoGrafico()
                {
                    Valor = _lista[i].ValorMayor,
                    FechaHora = _lista[i].HoraMayor
                };

                PuntoGrafico puntoMitad = new PuntoGrafico()
                {
                    Valor = _lista[i].ValorMitad,
                    FechaHora = _lista[i].HoraMitad
                };

                puntos.Add(puntoInicio);
                puntos.Add(puntoFinal);
                puntos.Add(puntoMenor);
                puntos.Add(puntoMayor);
                puntos.Add(puntoMitad);

            }

            puntos = puntos.Distinct().ToList();
            puntos = puntos.OrderBy(p => p.FechaHora).ToList();

            return puntos;
        }

        #endregion
    }
}