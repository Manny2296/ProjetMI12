package com.example.manuelfelipesr.mi12_example;

import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.content.Intent;
import android.content.pm.ActivityInfo;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.os.Build;
import android.support.annotation.Nullable;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import java.util.ArrayList;
import java.util.List;
import java.util.Set;
import java.util.UUID;

public class MainActivity extends AppCompatActivity  {

    private long last_update = 0, last_movement = 0;
    private float prevX = 0, prevY = 0, prevZ = 0;
    private float curX = 0, curY = 0, curZ = 0;
    Button btn_on,btn_off,btn_getdevices,btn_listdevices;
    private BluetoothAdapter BA;
    private ArrayAdapter<String> mBTArrayAdapter;
    private Set<BluetoothDevice>pairedDevices;
    BluetoothConnectionService mBluetoothConnection;
    public ArrayList<BluetoothDevice> mBTDevices = new ArrayList<>();
    private static final UUID MY_UUID_INSECURE =  UUID.fromString("8ce255c0-200a-11e0-ac64-0800200c9a66");
    ListView lv;
    BluetoothDevice mBTDevice;
    public static final String EXTRA_MESSAGE = "com.example.myfirstapp.MESSAGE";
    public void on(View v){
        if (!BA.isEnabled()) {
            Intent turnOn = new Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE);
            startActivityForResult(turnOn, 0);
            Toast.makeText(getApplicationContext(), "Turned on",Toast.LENGTH_LONG).show();
        } else {
            Toast.makeText(getApplicationContext(), "Already on", Toast.LENGTH_LONG).show();
        }
    }

    public void off(View v){
        BA.disable();
        Toast.makeText(getApplicationContext(), "Turned off" ,Toast.LENGTH_LONG).show();
    }


    public  void visible(View v){
        Intent getVisible = new Intent(BluetoothAdapter.ACTION_REQUEST_DISCOVERABLE);
        startActivityForResult(getVisible, 0);
    }


    public void list(View v){
        pairedDevices = BA.getBondedDevices();

        ArrayList list = new ArrayList();
        ArrayAdapter<String> adapter = new ArrayAdapter<>(this,android.R.layout.simple_list_item_1);;
        Toast.makeText(getApplicationContext(), "Showing Paired Devices" ,Toast.LENGTH_SHORT).show();
        for(BluetoothDevice bt : pairedDevices) {
        list.add(bt.getName());
        mBTDevices.add(bt);

        }

        adapter = new ArrayAdapter<String>(this, android.R.layout.simple_list_item_1, list);
        lv = (ListView) findViewById(R.id.listview1);
        lv.setAdapter(adapter);

        lv.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position,
                                    long id) {

                String deviceName = mBTDevices.get(position).getName();
                Toast.makeText(getApplicationContext(), deviceName + "Conecting ...", Toast.LENGTH_LONG).show();

                mBTDevice = mBTDevices.get(position);

                Intent intent = new Intent(getApplicationContext(),ConectActivity.class);
                //String message = "abc";
               intent.putExtra("mBTDevice",mBTDevice);
              // intent.putExtra("disp",mBTDevice.getName());
             //   mBluetoothConnection = new BluetoothConnectionService(MainActivity.this);
              // mBluetoothConnection.startClient(mBTDevice,MY_UUID_INSECURE);
                startActivity(intent);
            }
        });
    }
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        btn_on = (Button) findViewById(R.id.button1);
        btn_off=(Button)findViewById(R.id.button2);
        btn_getdevices=(Button)findViewById(R.id.button3);
        btn_listdevices=(Button)findViewById(R.id.button4);
        mBTArrayAdapter = new ArrayAdapter<String>(this,android.R.layout.simple_list_item_1);
        BA = BluetoothAdapter.getDefaultAdapter();
        //lv = (ListView)findViewById(R.id.listview1);
    }





}
