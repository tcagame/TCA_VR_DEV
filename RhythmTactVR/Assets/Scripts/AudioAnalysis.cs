using UnityEngine;
using System.Collections;

[ RequireComponent( typeof( AudioSource ) ) ]
public class AudioAnalysis : MonoBehaviour {

	private AudioSource _source;
	private AudioClip _clip;

	const int FRAME_LEN = 512;
	int datasize = 0;
	float[ ] samples;
	double[ ] frame = new double[ FRAME_LEN ];
	int N;
	double[ ] vol;
	double[ ] diff;
	float s = 0;

	double[ ] a;
	double[ ] b;
	double[ ] r;


	void Awake( ) {
		_source = GetComponent< AudioSource >( );
		_clip = _source.clip;

		datasize = _clip.samples * _clip.channels;
		samples = new float[ datasize ];
		_clip.GetData( samples, 0 );
		N = datasize / sizeof( float ) / _clip.channels / FRAME_LEN;

		s = _clip.frequency;//double(fmt->dwSamplesPerSec) / double(FRAME_LEN); // サンプリング周波数
		
		vol = getVollume( );
		diff = getDiff( );

		analysis( );	// 解析開始

		// ピーク解析
		int[ ] peak_x = new int[ 3 ];
		find_peak3( ref r, 240-60+1, ref peak_x );

		for ( int i = 0; i < 3; i++ ) {
			Debug.Log( "BPM[ " + i +" ] : " + peak_x[ i ] );
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void analysis( ) {

		// テンポ解析
		a = new double[ 240 - 60 + 1 ];
		b = new double[ 240 - 60 + 1 ];
		r = new double[ 240 - 60 + 1 ];
		for ( int bpm = 60; bpm <= 240; bpm++ ) {
			double a_sum = 0;
			double b_sum = 0;
			double f = ( double )bpm / 60;
			for ( int n = 0; n < N; n++ ) {
				double win = han_window( n, N );
				a_sum += diff[ n ] * Mathf.Cos( 2.0f * Mathf.PI * ( float )f * n / s ) * win;
				b_sum += diff[ n ] * Mathf.Sin( 2.0f * Mathf.PI * ( float )f * n / s ) * win;
				// 注意：窓関数を使用しないと端の影響で誤差が出る
			}
			double a_tmp = a_sum / N;
			double b_tmp = b_sum / N;
			a[bpm-60] = a_tmp;
			b[bpm-60] = b_tmp;
			r[bpm-60] = Mathf.Sqrt( ( float )power( a_tmp, b_tmp ) );
		}
	}

	double[ ] getVollume( ) {
	 // FFT用(音量にFFT係数の合計を使用する場合)
		double[ ] vol = new double[ N ];
		uint i = 0;
		int j = 0;
		int m = 0;
		while (i <= datasize/sizeof(short) && m < N)
		{
			frame[j] = samples[i];
			j++;

			if (j == FRAME_LEN)
			{
				// 音量(実効値)=sqrt(1/FRAME_LEN * Σframe[n]^2)
				double sum = 0;
				for (int n = 0; n < FRAME_LEN; n++)
				{
					sum += frame[n] * frame[n];
				}

				vol[m] = Mathf.Sqrt( ( float )sum / FRAME_LEN );
				m++;

				j = 0; // 次フレーム
			}

			i += 2;
		}

		return vol;
	}

	double[ ] getDiff( ) {
		// 音量差分(増加のみ)
		double[ ] diff = new double[ N ]; // 音量差分

		diff[0] = vol[0];
		for ( int i = 1; i < N; i++ ) {
			if ( vol[i] - vol[i-1] > 0 ) {
				diff[i] = vol[i] - vol[i-1];
			} else {
				diff[i] = 0;
			}
		}

		return diff;
	}

	double han_window( int i, int size ) {
		return 0.5 - 0.5 * Mathf.Cos( 2.0f * Mathf.PI * i / size );
	}

	double power( double re, double im ) {
		return re * re + im * im;
	}

	void find_peak3( ref double[ ] a, int size, ref int[ ] max_idx ) {
		max_idx[0] = -1;
		max_idx[1] = -1;
		max_idx[2] = -1;
		double dy = 0;
		for (int i = 1; i < size; ++i) {
			double dy_pre = dy;
			dy = a[i] - a[i-1];
			if (dy_pre > 0 && dy <= 0)
			{
				if (max_idx[0] < 0 || a[i-1] > a[max_idx[0]])
				{
					max_idx[2] = max_idx[1];
					max_idx[1] = max_idx[0];
					max_idx[0] = i-1;
				}
				else if (max_idx[1] < 0 || a[i-1] > a[max_idx[1]])
				{
					max_idx[2] = max_idx[1];
					max_idx[1] = i-1;
				}
				else if (max_idx[2] < 0 || a[i-1] > a[max_idx[2]])
				{
					max_idx[2] = i-1;
				}
			}
		}
	}
}

