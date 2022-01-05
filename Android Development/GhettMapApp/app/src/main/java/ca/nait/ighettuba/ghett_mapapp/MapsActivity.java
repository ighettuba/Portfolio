package ca.nait.ighettuba.ghett_mapapp;

import android.Manifest;
import android.content.DialogInterface;
import android.content.pm.PackageManager;
import android.location.Location;
import android.os.Bundle;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.FrameLayout;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;

import com.google.android.gms.location.FusedLocationProviderClient;
import com.google.android.gms.location.LocationServices;
import com.google.android.gms.maps.CameraUpdateFactory;
import com.google.android.gms.maps.GoogleMap;
import com.google.android.gms.maps.OnMapReadyCallback;
import com.google.android.gms.maps.SupportMapFragment;
import com.google.android.gms.maps.model.CameraPosition;
import com.google.android.gms.maps.model.LatLng;
import com.google.android.gms.maps.model.Marker;
import com.google.android.gms.maps.model.MarkerOptions;
import com.google.android.gms.tasks.OnCompleteListener;
import com.google.android.gms.tasks.Task;
import com.google.android.libraries.places.api.Places;
import com.google.android.libraries.places.api.model.Place;
import com.google.android.libraries.places.api.model.PlaceLikelihood;
import com.google.android.libraries.places.api.net.FindCurrentPlaceRequest;
import com.google.android.libraries.places.api.net.FindCurrentPlaceResponse;
import com.google.android.libraries.places.api.net.PlacesClient;

import java.util.Arrays;
import java.util.List;

//Activity will display a map showing devices current location
public class MapsActivity extends AppCompatActivity implements OnMapReadyCallback
{
    ////////////////////////VARIABLES

    private static final String TAG ="MapsActivity";

    // Objects used by App
    private GoogleMap ghettMap;
    private CameraPosition cameraPosition;
    // The entry point to the Places API
    private PlacesClient placesClient;

    // The entry point to the Fused Location Provider.
    private FusedLocationProviderClient fusedLocationProviderClient;

    //Create a default location (Nairobi Kenya) for the app to fall back on when permission is not granted
    private final LatLng defaultLocation = new LatLng(0.0236, 37.9062);
    private static final int  DEFAULT_ZOOM = 15;
    private static final int PERMISSIONS_REQUEST_ACCESS_FINE_LOCATION = 1;
    private boolean bLocPermissionGranted;

    //current geographical location of device
    private Location lastKnownLocation;

    // Keys for storing activity state.
    private static final String KEY_CAMERA_POSITION = "camera_position";
    private static final String KEY_LOCATION = "location";

    // Used for selecting the current place.
    private static final int M_MAX_ENTRIES = 12;
    private String[] likelyPlaceNames;
    private String[] likelyPlaceAddresses;
    private List[] likelyPlaceAttributions;
    private LatLng[] likelyPlaceLatLngs;


    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);

        //load saveInstanceState if previously saved
        if (savedInstanceState != null)
        {
            lastKnownLocation = savedInstanceState.getParcelable(KEY_LOCATION);
            cameraPosition = savedInstanceState.getParcelable(KEY_CAMERA_POSITION);
            Log.d(TAG,"savedInstanceState not null.");

        }

        //retrieve content view
        setContentView(R.layout.activity_maps);

        //Construct PlacesClient using Google API Key
        Places.initialize(getApplicationContext(),getString(R.string.google_maps_key));
        placesClient = Places.createClient(this);

        //Construct FusedLocationProviderClient
        fusedLocationProviderClient = LocationServices.getFusedLocationProviderClient(this);

        // Obtain the SupportMapFragment and get notified when the map is ready to be used.
        SupportMapFragment mapFragment = (SupportMapFragment) getSupportFragmentManager()
                .findFragmentById(R.id.map);
        mapFragment.getMapAsync(this);
        Log.d(TAG,"In OnCreate()");

    }// Closes onCreate()

    //save map camera position and device location on the map
    @Override
    protected void onSaveInstanceState( Bundle outState)
    {
        if (ghettMap != null)
        {
            outState.putParcelable(KEY_CAMERA_POSITION, ghettMap.getCameraPosition());
            outState.putParcelable(KEY_LOCATION,lastKnownLocation);
        }
        super.onSaveInstanceState(outState);
        Log.d(TAG,"Saved SavedInstanceState()");
    } //closes onSaveInstanceState()

    @Override
    public boolean onCreateOptionsMenu(Menu menu)
    {
        Log.d(TAG,"Menu Options Created()");
        getMenuInflater().inflate(R.menu.current_place_menu,menu);
        return true;

    }// CLoses onCreateOptionsMenu

    @Override
    public boolean onOptionsItemSelected(@NonNull MenuItem item)
    {
        if (item.getItemId() == R.id.option_get_place)
            showCurrentPlace();
        Log.d(TAG,"Item Selected()");
        return true;
    } //Closes onOptionsItemSelected



    /**
     * Manipulates the map once available.
     * This callback is triggered when the map is ready to be used.
     * This is where we can add markers or lines, add listeners or move the camera. In this case,
     * we just add a marker near Sydney, Australia.
     * If Google Play services is not installed on the device, the user will be prompted to install
     * it inside the SupportMapFragment. This method will only be triggered once the user has
     * installed Google Play services and returned to the app.
     */
    @Override
    public void onMapReady(GoogleMap googleMap)
    {
        ghettMap = googleMap;

        //Implement InfoWindowAdapter method to fill the get places list
        ghettMap.setInfoWindowAdapter(new GoogleMap.InfoWindowAdapter()
        {
            @Override
            public View getInfoWindow(Marker marker)
            {
                return null;
            }

            @Override
            public View getInfoContents(Marker marker)
            {
                //inflate layout for info  window, title, and snippet
                View infoWindow = getLayoutInflater().inflate(R.layout.custom_info_contents,(FrameLayout) findViewById(R.id.map), false);

                TextView title = infoWindow.findViewById(R.id.title);
                title.setText(marker.getTitle());
                TextView snippet = infoWindow.findViewById(R.id.snippet);
                title.setText(marker.getSnippet());

                return infoWindow;
            }
        });// CLoses getInfoContents()

        //Turn on Location layer and control on the map
        updateLocationUI();

        //get current device location
        getDeviceLocation();
        Log.d(TAG,"In onMapReady()");

    }// CLoses onMapReady

    private void getDeviceLocation()
    {
        //get the most recent location of the phone
        try
        {
            if(bLocPermissionGranted)
            {
                Task<Location> locationResult = fusedLocationProviderClient.getLastLocation();
                locationResult.addOnCompleteListener(this, new OnCompleteListener<Location>()
                {
                    @Override
                    public void onComplete(@NonNull Task<Location> task)
                    {
                        if (task.isSuccessful())
                        {
                            lastKnownLocation = task.getResult();
                            if (lastKnownLocation !=null)
                                ghettMap.moveCamera(CameraUpdateFactory
                                        .newLatLngZoom(new LatLng(
                                                        lastKnownLocation.getLatitude(),
                                                        lastKnownLocation.getLongitude()),
                                                        DEFAULT_ZOOM));
                        }
                        else
                        {
                            Log.d(TAG,"Default location displayed. Current location returned null");
                            Log.e(TAG, "Exception: %s,",task.getException());
                            ghettMap.moveCamera(CameraUpdateFactory
                                    .newLatLngZoom(defaultLocation,DEFAULT_ZOOM));
                            ghettMap.getUiSettings().setMyLocationButtonEnabled(false);
                        }
                    }
                });
            }
        }
        catch (SecurityException e)
        {
            Log.e("Exception: %s", e.getMessage(), e);
        }// Closes Try Catch
        Log.d(TAG,"In getDeviceLocation()");

    }// Closes getDeviceLocation()

    private void getLocationPermission()
    {
        //requests location permission for the users device
        //result is handled in a callback. SEE onRequestPermissionsResult method
                // check if permission has been given
        if (ContextCompat.checkSelfPermission(this.getApplicationContext(), Manifest.permission.ACCESS_FINE_LOCATION) == PackageManager.PERMISSION_GRANTED)
            //app has permission to use phone location data
            bLocPermissionGranted = true;
        else
            //ask user to provide permission
            ActivityCompat.requestPermissions(this, new String[]{Manifest.permission.ACCESS_FINE_LOCATION}, PERMISSIONS_REQUEST_ACCESS_FINE_LOCATION);
        Log.d(TAG,"In getLocationPermission()");

    }// Closes getLocationPermission
    
    //Handle request permissions result
    @Override
    public void onRequestPermissionsResult(int requestCode, @NonNull String[] permissions, @NonNull int[] grantResults)
    {
        bLocPermissionGranted = false;
        switch (requestCode)
        {
            case PERMISSIONS_REQUEST_ACCESS_FINE_LOCATION:
            {
                //if cancelled request result arrays are empty
                if (grantResults.length > 0 && grantResults[0] == PackageManager.PERMISSION_GRANTED)
                    bLocPermissionGranted = true;
            }
        }
        updateLocationUI();

        Log.d(TAG,"In onRequestPermissionsResult()");
    }// Closes onRequestPermissionsResult

    private void updateLocationUI()
    {
        if (ghettMap == null)
            return;
        try
        {
            if (bLocPermissionGranted)
            {
                ghettMap.setMyLocationEnabled(true);
                ghettMap.getUiSettings().setMyLocationButtonEnabled(true);
            }
            else
            {
                ghettMap.setMyLocationEnabled(false);
                ghettMap.getUiSettings().setMyLocationButtonEnabled(false);
                lastKnownLocation = null;
                getLocationPermission();                                        //request location permission from user
            }
        }
        catch (SecurityException e)
        {
            Log.e("Exception: %s",e.getMessage());
        }
        Log.d(TAG,"In updateLocationUI()");

    }//CLoses updateLocationUI()

    private void showCurrentPlace()
    {
        if (ghettMap == null)
            return;

        if (bLocPermissionGranted)
        {
            // Use fields (Places SDK) to define return data types
            List<Place.Field> placeFields = Arrays.asList(
                    Place.Field.NAME,
                    Place.Field.ADDRESS,
                    Place.Field.LAT_LNG);

            //Use builder to create a FindCurrentRequest
            FindCurrentPlaceRequest request = FindCurrentPlaceRequest.newInstance(placeFields);

            //Get Likely Places near the devices current location
            @SuppressWarnings("MissingPermission") final
            Task<FindCurrentPlaceResponse> placeResult =
                    placesClient.findCurrentPlace(request);

            //set addOnCompleteListener
            placeResult.addOnCompleteListener(new OnCompleteListener<FindCurrentPlaceResponse>()
            {
                @Override
                public void onComplete(@NonNull Task<FindCurrentPlaceResponse> task)
                {
                    if (task.isSuccessful() && task.getResult() !=null)
                    {
                        FindCurrentPlaceResponse likelyPlaces = task.getResult();
                        
                        //set counter to handle cases with less than 5 entries
                        int count;
                        if (likelyPlaces.getPlaceLikelihoods().size() < M_MAX_ENTRIES)
                            count = likelyPlaces.getPlaceLikelihoods().size();
                        else
                            count = M_MAX_ENTRIES;

                        //initialize arrays to store list of current places and their coordinates
                        int i = 0;
                        likelyPlaceNames=new String[count];
                        likelyPlaceAddresses = new String[count];
                        likelyPlaceAttributions = new List[count];
                        likelyPlaceLatLngs = new LatLng[count];

                        for (PlaceLikelihood placeLikelihood: likelyPlaces.getPlaceLikelihoods())
                        {
                            likelyPlaceNames[i] = placeLikelihood.getPlace().getName();
                            likelyPlaceAddresses[i] = placeLikelihood.getPlace().getAddress();
                            likelyPlaceAttributions[i] = placeLikelihood.getPlace().getAttributions();
                            likelyPlaceLatLngs[i] = placeLikelihood.getPlace().getLatLng();

                            i++;
                            if (i > (count - 1))
                                break;
                        }

                        //display likely places  and add marker at the selected location
                        MapsActivity.this.openPlacesDialog();
                    }
                    else
                        Log.i(TAG,"Exception: %s", task.getException());
                }// Closes onComplete()
            });//Closes OnCompleteListener()
        }
        else
        {
            Log.i(TAG,"User has not granted location permissions");

            //Add default marker
            ghettMap.addMarker(new MarkerOptions()
            .title(getString(R.string.default_info_title))
            .position(defaultLocation)
            .snippet(getString(R.string.default_info_snippet)));

            //prompt for permission
            getLocationPermission();
        }
        Log.d(TAG,"In showCurrentPlace()");
    }//Closes showCurrentPlace()

    private void openPlacesDialog()
    {
        //ask user to select place they are now.
        DialogInterface.OnClickListener listener = new DialogInterface.OnClickListener()
        {
            @Override
            public void onClick(DialogInterface dialog, int which)  //which contains the position of the item selected
            {
                //set marker with likelyhoodPlaces details from arrayList
                LatLng markerLatLng = likelyPlaceLatLngs[which];
                String markerSnippet = likelyPlaceAddresses[which];
                if (likelyPlaceAttributions[which] !=null)
                    markerSnippet = markerSnippet = "\n" + likelyPlaceAttributions[which];

                //Add marker for selected place- centers when clicked
                ghettMap.addMarker(new MarkerOptions()
                .title(likelyPlaceNames[which])
                .position(markerLatLng)
                .snippet(markerSnippet));

                //position camera over marker location
                ghettMap.moveCamera(CameraUpdateFactory
                        .newLatLngZoom(
                                markerLatLng,
                                DEFAULT_ZOOM));
            }// Closes onClick
        };// Closes OnClickListener

        //Display Dialog
        AlertDialog dialog = new AlertDialog.Builder(this)
                .setTitle(R.string.pick_place)
                .setItems(likelyPlaceNames,listener)
                .show();

        Log.d(TAG,"In openPlacesDialog()");
    } //closes openPlacesDialog
}