int[,] PythagorasTable(int size) {
    int[,] a = new int[size, size];
	for (int i = 0; i < size; i++) {
		for (int j = 0; j < size; j++) {
			a[i, j] = (i+1)*(j+1);
		}
	}
    return a;
}

int[,] VedicSquare(int size) {
	int[,] t = PythagorasTable(size);
	for (int i = 0; i < size; i++) {
		for (int j = 0; j < size; j++) {
			t[i, j] = Aoeu(t[i, j]);
		}
	}
	return t;
}

int Aoeu(int n) {
	int s = 0;

	while (n > 0) {
		s += n % 10;
		n /= 10;
	}
	
	if (s > 9) {
		return Aoeu(s);
	}

	return s;
}

string[,] Pattern(int[,] a, int n) {
	int l = a.GetLength(0);
	string[,] r = new string[l, l];
	for (int i = 0; i < l; i++) {
		for (int j = 0; j < l; j++) {
			if (a[i, j] == n) {
				r[i, j] = "■";
			} else {
				r[i, j] = "□";
			}
		}
	}
	return r;
}

void Print2DArrayString(string[,] a) {
	for (int i = 0; i < a.GetLength(0); i++) {
		for (int j = 0; j < a.GetLength(1); j++) {
			Console.Write(a[i, j] + " ");
		}
		Console.WriteLine();
	}
}

void Print2DArray(int[,] a) {
	for (int i = 0; i < a.GetLength(0); i++) {
		for (int j = 0; j < a.GetLength(1); j++) {
			Console.Write(a[i, j] + " ");
		}
		Console.WriteLine();
	}
}

Print2DArrayString(Pattern(VedicSquare(9), 1));
