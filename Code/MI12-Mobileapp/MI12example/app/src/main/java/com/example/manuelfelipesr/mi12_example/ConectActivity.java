package com.example.manuelfelipesr.mi12_example;

import android.bluetooth.BluetoothDevice;
import android.content.pm.ActivityInfo;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.os.Bundle;
import android.support.annotation.Nullable;
import android.support.v7.app.AppCompatActivity;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import java.nio.charset.Charset;
import java.util.List;
import java.util.UUID;

public class ConectActivity extends AppCompatActivity implements SensorEventListener {
    private long last_update = 0, last_movement = 0;
    private float prevX = 0, prevY = 0, prevZ = 0;
    private float curX = 0, curY = 0, curZ = 0;
    Button btnSend;
    TextView disp;
    TextView txt_x;
    BluetoothDevice mBTDevice;
    BluetoothConnectionService mBluetoothConnection;
    private static final UUID MY_UUID_INSECURE =  UUID.fromString("8ce255d0-200a-11e0-ac64-0800200c9a66");

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_conect);
        btnSend = (Button) findViewById(R.id.btn_send);


   if(getIntent().getExtras() != null){
       mBTDevice = (BluetoothDevice)getIntent().getExtras().get("mBTDevice");
       mBluetoothConnection = new BluetoothConnectionService(ConectActivity.this);
       mBluetoothConnection.startClient(mBTDevice,MY_UUID_INSECURE);

       Toast.makeText(getApplicationContext(), ""+ mBTDevice.getName(), Toast.LENGTH_SHORT).show();

   }



    }
    public void writee(View v){

        txt_x = (TextView)findViewById(R.id.txtAccX);
        byte[] bytes =   txt_x.getText().toString().getBytes(Charset.defaultCharset());
        try {
            mBluetoothConnection.write(bytes);
        }
        catch (Exception e){
            Toast.makeText(getApplicationContext(),e.toString(), Toast.LENGTH_LONG);
        }
    }
    @Override
    public void onSensorChanged(SensorEvent event) {
        synchronized (this) {
            long current_time = event.timestamp;

            curX = event.values[0];
            curY = event.values[1];
            curZ = event.values[2];
            if (prevX == 0 && prevY == 0 && prevZ == 0) {
                last_update = current_time;
                last_movement = current_time;
                prevX = curX;
                prevY = curY;
                prevZ = curZ;
            }
            long time_difference = current_time - last_update;
            if (time_difference > 0) {
                float movement = Math.abs((curX + curY + curZ) - (prevX - prevY - prevZ)) / time_difference;
                int limit = 1500;
                float min_movement = 1E-6f;
                if (movement > min_movement) {
                    if (current_time - last_movement >= limit) {
                        //Toast.makeText(getApplicationContext(), "Hay movimiento de " + movement, Toast.LENGTH_SHORT).show();
                    }
                    last_movement = current_time;
                }
                prevX = curX;
                prevY = curY;
                prevZ = curZ;
                last_update = current_time;
            }




            ((TextView)findViewById(R.id.txtAccX)).setText("Acelerómetro X: " + curX);
            ((TextView) findViewById(R.id.txtAccY)).setText("Acelerómetro Y: " + curY);
            ((TextView) findViewById(R.id.txtAccZ)).setText("Acelerómetro Z: " + curZ);
            ((TextView)findViewById(R.id.txt_paried)).setText(mBTDevice.getName());


        }
    }

    @Override
    protected void onStop() {
        SensorManager sm = (SensorManager) getSystemService(SENSOR_SERVICE);
        sm.unregisterListener(this);
        super.onStop();
    }

    @Override
    protected void onResume() {
        super.onResume();
        SensorManager sm = (SensorManager) getSystemService(SENSOR_SERVICE);
        List<Sensor> sensors = sm.getSensorList(Sensor.TYPE_ACCELEROMETER);
        if (sensors.size() > 0) {
            sm.registerListener(this, sensors.get(0), SensorManager.SENSOR_DELAY_GAME);
        }
    }

    @Override
    protected void onPostCreate(@Nullable Bundle savedInstanceState) {
        super.onPostCreate(savedInstanceState);
        setContentView(R.layout.activity_conect);
        this.setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_PORTRAIT);

    }

    @Override
    public void onAccuracyChanged(Sensor sensor, int accuracy) {

    }
}
