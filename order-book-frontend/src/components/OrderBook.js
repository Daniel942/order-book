import { useEffect, useState } from 'react';
import Highcharts from 'highcharts';
import HighchartsReact from 'highcharts-react-official';
import '../styles/order-book.scss';
import { useParams } from 'react-router-dom';

function OrderBook() {
    const [retrievedAt, setRetrievedAt] = useState(null);
    const [chartOptions, setChartOptions] = useState(null);

    const { id } = useParams("id");

    useEffect(() => {
        const url = id ? `https://localhost:7117/api/getorderbookbyid?id=${id}` : 'https://localhost:7117/api/getorderbook?currencypair=btceur';
        fetch(url)
            .then(response => {
                if (response.ok) {
                    return response.json();
                }

                throw response;
            })
            .then(data => {
                console.log(data);

                // Make timestamp presentable
                setRetrievedAt(new Date(data.timestamp * 1000).toLocaleString());

                setChartOptions({
                    chart: {
                        type: 'area'
                    },
                    title: {
                        text: 'Order book'
                    },
                    legend: {
                        enabled: false
                    },
                    tooltip: {
                        formatter: function () {
                            return `${this.series.name}: ${this.y} BTC at ${this.x} EUR`
                        },
                        valueDecimals: 8
                    },
                    plotOptions: {
                        area: {
                            fillOpacity: 0.3,
                            lineWidth: 1,
                            step: 'center'
                        }
                    },
                    series: [
                        {
                            name: 'Bids',
                            data: data.bids.map(bid => bid.map(v => parseFloat(v))).reverse(), // Highcharts library needs float values to work properly. Reverse bids so they are ascending
                            color: '#1F8787'
                        },
                        {
                            name: 'Asks',
                            data: data.asks.map(ask => ask.map(v => parseFloat(v))), // Highcharts library needs float values to work properly.
                            color: '#DB3E3D'
                        }
                    ],
                    xAxis: {
                        title: {
                            text: 'Price'
                        }
                    },
                    yAxis: {
                        title: {
                            text: 'Amount'
                        }
                    }
                });
            })
            .catch(error => {
                console.error(error);
            });
        }, []);

    return (
        <>
            <p>Retrieved at: {retrievedAt}</p>

            {
                chartOptions && <HighchartsReact
                    highcharts={Highcharts}
                    options={chartOptions}
                />
            }
        </>
    );
}

export default OrderBook;